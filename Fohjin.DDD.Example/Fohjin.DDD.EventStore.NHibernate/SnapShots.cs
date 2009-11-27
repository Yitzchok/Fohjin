using System;
using FluentNHibernate.Mapping;

namespace Fohjin.DDD.EventStore.NHibernate
{
    public class SnapShots
    {
        public virtual Guid EventProviderId { get; set; }
        public virtual byte[] SnapShot { get; set; }
        public virtual int Version { get; set; }
    }

    public class SnapShotsMap : ClassMap<SnapShots>
    {
        public SnapShotsMap()
        {
            Id(c => c.EventProviderId).GeneratedBy.Assigned();
            Map(c => c.SnapShot).Not.Nullable();
            Map(c => c.Version).Not.Nullable();
        }
    }
}