using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Fohjin.DDD.Reporting.Infrastructure;

namespace Fohjin.DDD.Configuration.Castle
{
    public class ReportingRegistry
    {
        public ReportingRegistry(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISqlCreateBuilder>().ImplementedBy<SqlCreateBuilder>(),
                Component.For<ISqlInsertBuilder>().ImplementedBy<SqlInsertBuilder>(),
                Component.For<ISqlSelectBuilder>().ImplementedBy<SqlSelectBuilder>(),
                Component.For<ISqlUpdateBuilder>().ImplementedBy<SqlUpdateBuilder>(),
                Component.For<ISqlDeleteBuilder>().ImplementedBy<SqlDeleteBuilder>()
                );
        }
    }
}