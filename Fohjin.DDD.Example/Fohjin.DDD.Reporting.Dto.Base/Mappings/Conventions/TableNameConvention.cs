using FluentNHibernate.Conventions;

namespace Fohjin.DDD.Reporting.Dto.Base.Mappings.Conventions
{
    public class TableNameConvention : IClassConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IClassInstance instance)
        {
            instance.Table(Inflector.Net.Inflector.Pluralize(instance.EntityType.Name));
        }
    }
}