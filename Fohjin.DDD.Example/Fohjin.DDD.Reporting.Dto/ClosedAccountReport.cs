using System;

namespace Fohjin.DDD.Reporting.Dto
{
    public class ClosedAccountReport : AccountReport
    {
        protected ClosedAccountReport()
        {
        }

        public ClosedAccountReport(Guid id, Guid clientDetailsId, string accountName, string accountNumber)
            : base(id, clientDetailsId, accountName, accountNumber)
        {
        }
    }
}