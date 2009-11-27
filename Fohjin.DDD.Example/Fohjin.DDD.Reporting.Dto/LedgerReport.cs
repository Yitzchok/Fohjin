using System;
using Fohjin.DDD.Reporting.Dto.Base.Model;

namespace Fohjin.DDD.Reporting.Dto
{
    public class LedgerReport : Entity
    {
        protected LedgerReport()
        {
        }

        public virtual Guid AccountDetailsReportId { get; private set; }
        public virtual string Action { get; private set; }
        public virtual decimal Amount { get; private set; }

        public LedgerReport(Guid id, Guid accountDetailsId, string action, decimal amount)
        {
            Id = id;
            AccountDetailsReportId = accountDetailsId;
            Action = action;
            Amount = amount;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1:C}", Action, Amount);
        }
    }
}