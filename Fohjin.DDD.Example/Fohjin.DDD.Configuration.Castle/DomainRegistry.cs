using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fohjin.DDD.Bus;
using Fohjin.DDD.Bus.Direct;
using Fohjin.DDD.EventStore;
using Fohjin.DDD.EventStore.Storage;
using IUnitOfWork = Fohjin.DDD.EventStore.IUnitOfWork;

namespace Fohjin.DDD.Configuration.Castle
{
    public class DomainRegistry
    {
        public DomainRegistry(IWindsorContainer container)
        {
            container.Register(
               Component.For<IBus>().ImplementedBy<DirectBus>().LifeStyle.PerThread,
               Component.For<Bus.IUnitOfWork>().Forward<IBus>(),
               Component.For<IRouteMessages>().ImplementedBy<MessageRouter>().LifeStyle.Singleton,
               Component.For<IFormatter>().ImplementedBy<BinaryFormatter>(),

               Component.For<IIdentityMap<IDomainEvent>>().ImplementedBy<EventStoreIdentityMap<IDomainEvent>>(),
               Component.For<IEventStoreUnitOfWork<IDomainEvent>>().ImplementedBy<EventStoreUnitOfWork<IDomainEvent>>()
                   .LifeStyle.Singleton,

               Component.For<IUnitOfWork>().Forward<IEventStoreUnitOfWork<IDomainEvent>>(),

               Component.For<IDomainRepository<IDomainEvent>>().ImplementedBy<DomainRepository<IDomainEvent>>()
               .LifeStyle.Singleton);
        }
    }
}