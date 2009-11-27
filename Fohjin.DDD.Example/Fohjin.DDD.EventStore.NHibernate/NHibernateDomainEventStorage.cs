using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Fohjin.DDD.EventStore.SQLite;
using Fohjin.DDD.EventStore.Storage;
using Fohjin.DDD.EventStore.Storage.Memento;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Fohjin.DDD.EventStore.NHibernate
{
    public class NHibernateDomainEventStorage<TDomainEvent> : IDomainEventStorage<TDomainEvent> where TDomainEvent : IDomainEvent
    {
        readonly ISessionFactory sessionFactory;

        readonly IFormatter _formatter;

        public NHibernateDomainEventStorage(ISessionFactory databaseSession, IFormatter formatter)
        {
            this.sessionFactory = databaseSession;
            _formatter = formatter;
        }

        public IEnumerable<TDomainEvent> GetAllEvents(Guid eventProviderId)
        {
            IEnumerable<TDomainEvent> domainEvents = null;
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        domainEvents = session.CreateCriteria<EventProviders>().Add(Restrictions.Eq("EventProviderId", eventProviderId))
                            .List<TDomainEvent>();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return domainEvents;
        }

        public IEnumerable<TDomainEvent> GetEventsSinceLastSnapShot(Guid eventProviderId)
        {
            var snapShot = GetSnapShot(eventProviderId);

            var snapShotVersion = snapShot != null
                                 ? snapShot.Version
                                 : -1;


            var domainEvents = new List<TDomainEvent>();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var events = from @event in session.Linq<Events>()
                                     where @event.EventProviderId == eventProviderId && @event.Version > snapShotVersion
                                     orderby @event.Version ascending
                                     select @event;

                        foreach (var @event in events)
                        {
                            domainEvents.Add(Deserialize<TDomainEvent>(@event.Event));
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return domainEvents;
        }

        public int GetEventCountSinceLastSnapShot(Guid eventProviderId)
        {
            int count;
            var snapShot = GetSnapShot(eventProviderId);

            var snapShotVersion = snapShot != null
                                 ? snapShot.Version
                                 : 0;

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        count = session.CreateCriteria<Events>()
                            .Add(Restrictions.Eq("EventProviderId", eventProviderId))
                            .Add(Restrictions.Gt("Version", snapShotVersion))
                            .SetProjection(Projections.Count("EventProviderId")).UniqueResult<int>();
                        session.Flush();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return count;
        }

        public void Save(IEventProvider<TDomainEvent> eventProvider)
        {
            if (!_isRunningWithinTransaction)
                throw new Exception("Opperation is not running within a transaction");

            var version = GetEventProviderVersion(eventProvider, session, transaction);

            if (version != eventProvider.Version)
                throw new ConcurrencyViolationException();

            foreach (var domainEvent in eventProvider.GetChanges())
            {
                SaveEvent(domainEvent, eventProvider, transaction);
            }

            eventProvider.UpdateVersion(eventProvider.Version + eventProvider.GetChanges().Count());
            UpdateEventProviderVersion(eventProvider, session, transaction);
        }

        public ISnapShot GetSnapShot(Guid eventProviderId)
        {
            ISnapShot snapshot = null;

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        var bytes = (from snapShot in session.Linq<SnapShots>()
                                     where snapShot.EventProviderId == eventProviderId && snapShot.Version != -1
                                     select snapShot.SnapShot).SingleOrDefault();

                        if (bytes != null)
                            snapshot = Deserialize<ISnapShot>(bytes);

                        //snapshot = session.CreateCriteria<SnapShots>()
                        //    .Add(Restrictions.Eq("EventProviderId", eventProviderId))
                        //    .Add(Restrictions.Not(Restrictions.Eq()))
                        //    .List<TDomainEvent>();

                        session.Flush();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return snapshot;
        }

        public void SaveShapShot(IEventProvider<TDomainEvent> entity)
        {
            StoreSnapShot(new SnapShot(entity.Id, entity.Version, ((IOrginator)entity).CreateMemento()));
        }

        bool _isRunningWithinTransaction;
        ISession session;
        ITransaction transaction;
        public void BeginTransaction()
        {
            session = sessionFactory.OpenSession();
            transaction = session.BeginTransaction();
            _isRunningWithinTransaction = true;
        }

        public void Commit()
        {
            _isRunningWithinTransaction = false;
            session.Flush();
            transaction.Commit();
        }

        public void Rollback()
        {
            _isRunningWithinTransaction = false;
            transaction.Rollback();
        }

        private void SaveEvent(TDomainEvent domainEvent, IEventProvider<TDomainEvent> eventProvider, ITransaction transaction)
        {
            session.Save(new Events
                             {
                                 Id = domainEvent.Id,
                                 EventProviderId = eventProvider.Id,
                                 Event = Serialize(domainEvent),
                                 Version = domainEvent.Version
                             });
        }

        private void StoreSnapShot(ISnapShot snapShot)
        {
            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.SaveOrUpdateCopy(new SnapShots
                        {
                            EventProviderId = snapShot.EventProviderId,
                            SnapShot = Serialize(snapShot),
                            Version = snapShot.Version
                        });
                        session.Flush();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static void UpdateEventProviderVersion(IEventProvider<TDomainEvent> eventProvider, ISession session, ITransaction transaction)
        {
            session.CreateQuery("update EventProviders set Version = :version where EventProviderId = :eventProviderId")
                .SetInt32("version", eventProvider.Version)
                .SetGuid("eventProviderId", eventProvider.Id).ExecuteUpdate();

            //session.Flush();
        }

        private static int GetEventProviderVersion(IEventProvider<TDomainEvent> eventProvider, ISession session, ITransaction transaction)
        {
            var eventProviderFromDB = session.Get<EventProviders>(eventProvider.Id);

            if (eventProviderFromDB != null)
                return eventProviderFromDB.Version;

            session.Save(new EventProviders
            {
                EventProviderId = eventProvider.Id,
                Type = eventProvider.GetType().FullName,
                Version = eventProvider.Version
            });

            return eventProvider.Version;
        }

        private byte[] Serialize(object theObject)
        {
            using (var memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, theObject);
                return memoryStream.ToArray();
            }
        }

        private TType Deserialize<TType>(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                return (TType)_formatter.Deserialize(memoryStream);
            }
        }
    }
}