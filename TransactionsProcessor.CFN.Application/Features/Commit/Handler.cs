using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Commit
{
    public partial class Commit
    {
        public class Command : IRequest<Result>
        {
            public List<ParseModel> ParseTransactions { get; set; }
        }

        public class Result
        {
            public List<ParseModel> ParseModel { get; set; }

            public List<BatchDetails> BatchDetailsList { get; set; }

            public List<int> TransactionIdsForRollback { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var billingResponse = BillingRequest(request);

                return Task.FromResult(new Result());
            }

            private BillingResponse BillingRequest(Command command)
            {
                return new BillingResponse();
            }

            private List<int> ReconcileInsertedTransactions(Dictionary<int, ParseModel> cfnFileDictionary, Dictionary<int, int> insertedTransactionsDictionary)
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
}
