using System;
using System.Collections.Generic;
using Fohjin.DDD.Reporting.Dto.Base.Model;

namespace Fohjin.DDD.Reporting.Dto
{
    public class ClientDetailsReport : Entity
    {
        protected ClientDetailsReport()
        {
        }

        public virtual IEnumerable<AccountReport> Accounts { get; private set; }
        public virtual IEnumerable<ClosedAccountReport> ClosedAccounts { get; private set; }
        public virtual string ClientName { get; private set; }
        public virtual string Street { get; private set; }
        public virtual string StreetNumber { get; private set; }
        public virtual string PostalCode { get; private set; }
        public virtual string City { get; private set; }
        public virtual string PhoneNumber { get; set; }

        public ClientDetailsReport(Guid id, string clientName, string street, string streetNumber, string postalCode, string city, string phoneNumber)
        {
            Id = id;
            Accounts = new List<AccountReport>();
            ClosedAccounts = new List<ClosedAccountReport>();
            ClientName = clientName;
            Street = street;
            StreetNumber = streetNumber;
            PostalCode = postalCode;
            City = city;
            PhoneNumber = phoneNumber;
        }
    }
}