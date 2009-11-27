using System;
using FluentNHibernate.Mapping;

namespace Fohjin.DDD.EventStore.NHibernate
{
    public class Events
    {
        public virtual Guid Id { get; set; }
        public virtual Guid EventProviderId { get; set; }
        public virtual byte[] Event { get; set; }
        public virtual int Version { get; set; }
    }

    public class EventsMap : ClassMap<Events>
    {
        public EventsMap()
        {
            Id(c => c.Id).GeneratedBy.Assigned();
            Map(c => c.Event).Not.Nullable();
            Map(c => c.EventProviderId).Not.Nullable();

            Map(c => c.Version).Not.Nullable();
            //Version(c => c.Version);
        }
    }
}