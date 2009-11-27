using System;
using System.Collections.Generic;
using Fohjin.DDD.Reporting.Dto.Base.Model;

namespace Fohjin.DDD.Reporting.Dto
{
    public class AccountDetailsReport : Entity
    {
        protected AccountDetailsReport()
        {
        }

        public virtual Guid ClientReportId { get; private set; }
        public virtual IEnumerable<LedgerReport> Ledgers { get; private set; }
        public virtual string AccountName { get; private set; }
        public virtual decimal Balance { get; set; }
        public virtual string AccountNumber { get; private set; }

        public AccountDetailsReport(Guid id, Guid clientId, string accountName, decimal balance, string accountNumber)
        {
            Id = id;
            ClientReportId = clientId;
            Ledgers = new List<LedgerReport>();
            AccountName = accountName;
            Balance = balance;
            AccountNumber = accountNumber;
        }
    }
}