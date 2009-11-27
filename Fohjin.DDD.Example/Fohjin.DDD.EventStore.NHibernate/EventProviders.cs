using System;
using FluentNHibernate.Mapping;

namespace Fohjin.DDD.EventStore.NHibernate
{
    public class EventProviders
    {
        public virtual Guid EventProviderId { get; set; }
        public virtual string Type { get; set; }
        public virtual int Version { get; set; }
    }

    public class EventProvidersMap : ClassMap<EventProviders>
    {
        public EventProvidersMap()
        {
            Id(c => c.EventProviderId).GeneratedBy.Assigned();
            Map(c => c.Type).Length(250).Not.Nullable();
            Map(c => c.Version).Not.Nullable();
        }
    }
}