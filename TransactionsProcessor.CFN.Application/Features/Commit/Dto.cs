using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionsProcessor.CFN.Application.Features.Commit
{
    public partial class Commit
    {
        public class BillingResponse
        {
            public Dictionary<int, int> DeliveryIdDictionary { get; set; }

            public List<BatchDetails> BatchDetailsList { get; set; }
        }

        public class BatchDetails
        {
            public int BatchNumber { get; set; }

            public int BatchCompanyId { get; set; }
        }
    }
}
