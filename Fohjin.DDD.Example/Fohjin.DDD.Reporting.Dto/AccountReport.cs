using System;
using Fohjin.DDD.Reporting.Dto.Base.Model;

namespace Fohjin.DDD.Reporting.Dto
{
    public class AccountReport : Entity
    {
        protected AccountReport()
        {
        }

        public virtual Guid ClientDetailsReportId { get; private set; }
        public virtual string AccountName { get; private set; }
        public virtual string AccountNumber { get; private set; }

        public AccountReport(Guid id, Guid clientDetailsId, string accountName, string accountNumber)
        {
            Id = id;
            ClientDetailsReportId = clientDetailsId;
            AccountName = accountName;
            AccountNumber = accountNumber;
        }

        public override string ToString()
        {
            return string.Format("{0} - ({1})", AccountNumber, AccountName);
        }
    }
}