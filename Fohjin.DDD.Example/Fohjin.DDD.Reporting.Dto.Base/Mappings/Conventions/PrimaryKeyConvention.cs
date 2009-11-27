using FluentNHibernate.Conventions;

namespace Fohjin.DDD.Reporting.Dto.Base.Mappings.Conventions
{
    public class PrimaryKeyConvention : IIdConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IIdentityInstance instance)
        {
            instance.Column(instance.EntityType.Name + "Id");
            instance.GeneratedBy.Assigned();
        }
    }
}