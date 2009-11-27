using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fohjin.DDD.CommandHandlers;
using Fohjin.DDD.EventHandlers;
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

            container.Register(AllTypes.Of(typeof(ICommandHandler<>))
               .FromAssembly(typeof(CreateClientCommandHandler).Assembly)
               .Where(t => !t.IsAbstract || !t.IsInterface)
               .Configure(c => c.Named(c.Name)));

            container.Register(AllTypes.Of(typeof(IEventHandler<>))
                .FromAssembly(typeof(ClientCreatedEventHandler).Assembly)
                .Where(t => !t.IsAbstract || !t.IsInterface)
                .Configure(c => c.Named(c.Name)));


            container.Register(
                Component.For<IReceiveMoneyTransfers>().ImplementedBy<MoneyReceiveService>(),
                Component.For<ISendMoneyTransfer>().ImplementedBy<MoneyTransferService>()
                );

            RegisterCommandHandlersInMessageRouter.BootStrap();
            RegisterEventHandlersInMessageRouter.BootStrap();
        }
    }
}