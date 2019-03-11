using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class ImportTransactions
    {
        public class Command : IRequest<Result>
        {
            public Dictionary<int, CfnFileModel> CfnFileDictionary { get; set; }

            public Dictionary<int, CfnBillingModel> BillingDictionary { get; set; }
        }

        public class Result
        {
            public Dictionary<int, CfnFileModel> BillingDictionary { get; set; }

            public List<int> TransactionIdsForRollback { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var billingResponse = await BillingRequest(request);

                var insertedTransactionsInCaseOfRollback = ReconcileInsertedTransactions(request.CfnFileDictionary, billingResponse.DeliveryIdDictionary);

                return new Result
                {
                    BillingDictionary = request.CfnFileDictionary,
                    TransactionIdsForRollback = insertedTransactionsInCaseOfRollback
                };
            }

            private async Task<BillingResponse> BillingRequest(Command command)
            {
                return new BillingResponse();
            }

            private List<int> ReconcileInsertedTransactions(Dictionary<int, CfnFileModel> cfnFileDictionary, Dictionary<int, int> insertedTransactionsDictionary)
            {
                var fallbackForInsertedTransactions = new List<int>();

                foreach (var billingResponsePair in insertedTransactionsDictionary)
                {
                    if (cfnFileDictionary.ContainsKey(billingResponsePair.Key))
                    {
                        cfnFileDictionary[billingResponsePair.Key].DeliveryId = billingResponsePair.Value;
                        fallbackForInsertedTransactions.Add(billingResponsePair.Value);
                    }
                    else
                    {
                        // TODO: Log this! 
                    }
                }

                return fallbackForInsertedTransactions;
            }
        }

    }

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
