using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fohjin.DDD.Services;

namespace Fohjin.DDD.Configuration.Castle
{
    public class CastleBootStraper
    {
        public void BootStrapTheApplication(IWindsorContainer container)
        {
            DomainDatabaseBootStrapper.BootStrap();
            ReportingDatabaseBootStrapper.BootStrap();

            new DomainRegistry(container);
            new ReportingRegistry(container);


            container.Register(
                Component.For<IReceiveMoneyTransfers>().ImplementedBy<MoneyReceiveService>(),
                Component.For<ISendMoneyTransfer>().ImplementedBy<MoneyTransferService>()
                );

            RegisterCommandHandlersInMessageRouter.BootStrap();
            RegisterEventHandlersInMessageRouter.BootStrap();
        }
    }
}