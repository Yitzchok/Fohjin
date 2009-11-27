using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Fohjin.DDD.EventStore;
using Fohjin.DDD.EventStore.NHibernate;
using Fohjin.DDD.EventStore.Storage;
using Fohjin.DDD.Reporting;
using Fohjin.DDD.Reporting.Dto;
using Fohjin.DDD.Reporting.Dto.Base.Mappings;
using Fohjin.DDD.Reporting.Infrastructure;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Fohjin.DDD.BankApplication
{
    public class BootstrapNHibernate
    {
        public BootstrapNHibernate(IWindsorContainer container)
        {
            SetupDomainDatabase(container);
            SetupReportingDatabase(container);
        }

        private const string PROXY_FACTORY = "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";

        protected void SetupDomainDatabase(IWindsorContainer container)
        {
            var domainSessionFactory = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile("domainDataBase.db3")
                              .ProxyFactoryFactory(PROXY_FACTORY))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<EventsMap>())
                .ExposeConfiguration(c => new SchemaUpdate(c).Execute(false, true))
                .BuildSessionFactory();

            container.Register(
                Component.For<ISessionFactory>().Named("DomainSessionFactory").Instance(domainSessionFactory),

                Component.For<IDomainEventStorage<IDomainEvent>>()
                    .ImplementedBy<NHibernateDomainEventStorage<IDomainEvent>>()
                    .ServiceOverrides(ServiceOverride.ForKey("databaseSession").Eq("DomainSessionFactory"))
                );
        }

        protected void SetupReportingDatabase(IWindsorContainer container)
        {
            var reportingSessionFactory = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile("reportingDataBase.db3")
                              .ProxyFactoryFactory(PROXY_FACTORY)
                              .UseOuterJoin)
                .Mappings(m => m.AutoMappings.Add(() => new AutoPersistenceModelGenerator().Generate(typeof(ClientReport).Assembly)))
                .ExposeConfiguration(c => new SchemaUpdate(c).Execute(false, true))
                .BuildSessionFactory();


            container.Register(
                Component.For<ISessionFactory>().Named("ReportingSessionFactory").Instance(reportingSessionFactory),
                Component.For<IReportingRepository>().ImplementedBy<NHibernateReportingRepository>()
                    .ServiceOverrides(ServiceOverride.ForKey("databaseSession").Eq("ReportingSessionFactory"))
                );
        }
    }
}