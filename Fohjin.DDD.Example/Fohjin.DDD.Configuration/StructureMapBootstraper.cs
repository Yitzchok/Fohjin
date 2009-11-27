using System;
using Fohjin.DDD.Services;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Fohjin.DDD.Configuration
{
    public class StructureMapBootstraper
    {
        public void BootStrapTheApplication(params Registry[] otherRegistryClasses)
        {
            DomainDatabaseBootStrapper.BootStrap();
            ReportingDatabaseBootStrapper.BootStrap();

            ObjectFactory.Initialize(x =>
            {
                x.AddOtherRegistry(otherRegistryClasses);
                x.AddRegistry<DomainRegistry>();
                x.AddRegistry<ReportingRegistry>();
                x.AddRegistry<ServicesRegister>();
            });

            ObjectFactory.AssertConfigurationIsValid();

            RegisterCommandHandlersInMessageRouter.BootStrap();
            RegisterEventHandlersInMessageRouter.BootStrap();
        }
    }

    public static class RegistryClass
    {
        public static void AddOtherRegistry(this IInitializationExpression expression, params Registry[] registryClasses)
        {
            foreach (var registry in registryClasses)
            {
                expression.AddRegistry(registry);
            }
        }
    }
}