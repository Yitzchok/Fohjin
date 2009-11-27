using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;

namespace Fohjin.DDD.Reporting.Infrastructure
{
    public class NHibernateReportingRepository : IReportingRepository
    {
        public ISessionFactory DatabaseSession { get; set; }
        //readonly ISession session;

        public NHibernateReportingRepository(ISessionFactory databaseSession)
        {
            DatabaseSession = databaseSession;
            //session = databaseSession.OpenSession();
        }

        public IEnumerable<TDto> GetByExample<TDto>(object example) where TDto : class
        {
            using (var session = DatabaseSession.OpenSession())
            {
                using (session.BeginTransaction())
                {
                    if (example == null)
                        return session.CreateCriteria<TDto>().List<TDto>();

                    return session
                        .CreateCriteria<TDto>().Add(Example.Create(example)).List<TDto>();

                }
            }
        }

        public void Save<TDto>(TDto dto) where TDto : class
        {
            using (var session = DatabaseSession.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Save(dto);
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

        public void Update<TDto>(object update, object where) where TDto : class
        {
            using (var session = DatabaseSession.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Update(update, where); transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Delete<TDto>(object example) where TDto : class
        {
            using (var session = DatabaseSession.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    try
                    {
                        session.Delete(example); transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}