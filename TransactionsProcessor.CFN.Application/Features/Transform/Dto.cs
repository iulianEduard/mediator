using System.Collections.Generic;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Transform
{
    public partial class Transform
    { 
        public class Context
        {
            public List<ParseModel> ParseTransactions { get; set; }

            public List<BillingModel> BillngTransactions { get; set; }

            public ICfnDatabase Database { get; set; }
        }

        public class FuelType
        {
            public int Id { get; set; }

            public int CFNProductCode { get; set; }

            public int FuelTypeId { get; set; }
        }

        public class BatchConfiguration
        {
            public int Id { get; set; }

            public int SiteId { get; set; }

            public int TruckId { get; set; }

            public int DriverId { get; set; }

            public int BatchCompanyId { get; set; }

            public bool? DieselOwned { get; set; }
        }

        public class CustomerType
        {
            public int ForeignCustomerId { get; set; }

            public int UnknownCustomerId { get; set; }
        }

        public class Customer
        {
            public string CardId { get; set; }

            public int CustomerId { get; set; }

            public string NameOnCard { get; set; }
        }

        public class Constants
        {
            public const int InvalidBatchNumber = -666;
        }
    }
}
