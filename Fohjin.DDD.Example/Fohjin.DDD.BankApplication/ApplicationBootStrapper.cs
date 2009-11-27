using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fohjin.DDD.Configuration;
using Fohjin.DDD.Configuration.Castle;

namespace Fohjin.DDD.BankApplication
{
    public class ApplicationBootStrapper
    {
        public static void BootStrap()
        {

        }

        public void BootstrapWithStructureMap()
        {
            new StructureMapBootstraper().BootStrapTheApplication(new StructureMapApplicationRegistry());

        }

        public void BootstrapWithCastle()
        {
            IWindsorContainer container = new WindsorContainer();

            new CastleBootStraper().BootStrapTheApplication(container);

            new CastleApplicationRegistry(container);
        }
    }

    public class BootstrapNHibernate
    {
        public BootstrapNHibernate()
        {
        }
    }
}