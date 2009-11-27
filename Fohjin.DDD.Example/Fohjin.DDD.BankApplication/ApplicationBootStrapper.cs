using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fohjin.DDD.Configuration;
using Fohjin.DDD.Configuration.Castle;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace Fohjin.DDD.BankApplication
{
    public class ApplicationBootStrapper
    {
        public static void BootStrap()
        {
            new ApplicationBootStrapper().BootstrapWithCastle();
        }

        public void BootstrapWithStructureMap()
        {

            new StructureMapBootstraper().BootStrapTheApplication(new StructureMapApplicationRegistry());
            
            IContainer container = new Container();
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(container));

        }

        public void BootstrapWithCastle()
        {
            IWindsorContainer container = new WindsorContainer();
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocatorAdapter(container));

            container.Register(Component.For<IWindsorContainer>().Instance(container));

            new BootstrapNHibernate(container);
            new CastleBootStraper().BootStrapTheApplication(container);

            new CastleApplicationRegistry(container);
        }
    }
}