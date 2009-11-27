using System;
using System.Linq;
using System.Reflection;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;
using Fohjin.DDD.Reporting.Dto.Base.Mappings.Conventions;
using Fohjin.DDD.Reporting.Dto.Base.Model;
using Entity = FluentNHibernate.Data.Entity;

namespace Fohjin.DDD.Reporting.Dto.Base.Mappings
{
    [CLSCompliant(false)]
    public interface IAutoPersistenceModelGenerator
    {
        AutoPersistenceModel Generate(Assembly reportAssembly);
    }

    public class AutoPersistenceModelGenerator : IAutoPersistenceModelGenerator
    {

        public AutoPersistenceModel Generate(Assembly reportAssembly)
        {
            var mappings = new AutoPersistenceModel();
            mappings.AddEntityAssembly(reportAssembly).Where(GetAutoMappingFilter);
            mappings.Conventions.Setup(GetConventions());
            mappings.Setup(GetSetup());
            mappings.IgnoreBase<Entity>();
            mappings.IgnoreBase(typeof(EntityWithTypedId<>));
            mappings.UseOverridesFromAssemblyOf<AutoPersistenceModelGenerator>();
            return mappings;
        }

        private Action<AutoMappingExpressions> GetSetup()
        {
            return c =>
                {
                    c.FindIdentity = type => type.Name == "Id";
                    c.GetComponentColumnPrefix = property => String.Empty;// property.Name;
                    c.IsBaseType = IsBaseTypeConvention;
                    c.IsComponentType = IsComponentTypeConvention;
                };
        }

        private Action<IConventionFinder> GetConventions()
        {
            return c =>
                {
                    c.Add<PrimaryKeyConvention>();
                    c.Add<HasManyConvention>();
                    c.Add<TableNameConvention>();
                    //c.Add<ValueObjectConvention>();
                    c.Add(FluentNHibernate.Conventions.Helpers.DefaultLazy.Never());
                };
        }

        /// <summary>
        /// Provides a filter for only including types which inherit from the IEntityWithTypedId interface.
        /// </summary>
        private bool GetAutoMappingFilter(Type t)
        {
            return t.GetInterfaces().Any(x =>
                                         x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntityWithTypedId<>));
        }

        private bool IsBaseTypeConvention(Type arg)
        {
            bool derivesFromEntity = arg == typeof(Entity);
            bool derivesFromEntityWithTypedId = arg.IsGenericType &&
                                                (arg.GetGenericTypeDefinition() == typeof(EntityWithTypedId<>));

            return derivesFromEntity || derivesFromEntityWithTypedId;
        }

        private bool IsComponentTypeConvention(Type arg)
        {
            return arg.BaseType == typeof(ValueObject);
        }
    }
}