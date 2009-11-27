using System;
using Fohjin.DDD.Reporting.Dto.Base.Model;

namespace Fohjin.DDD.Reporting.Dto
{
    public class ClientReport : Entity
    {
        protected ClientReport()
        {
        }

        public virtual string Name { get; set; }

        public ClientReport(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}