using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Transform
{
    public partial class Transform
    {
        public class Command : IRequest<Result>
        {
            public List<ParseModel> ParseTransactions { get; set; }
        }

        public class Result
        {
            public List<ParseModel> ParseTransactions { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnDatabase _database;

            public Handler(ICfnDatabase database)
            {
                _database = database;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var context = new Context
                {
                    ParseTransactions = request.ParseTransactions,
                    Database = _database
                };

                Steps.Prepare.Handle(context);
                await Steps.ChangeFuelTypes.Handle(context);
                await Steps.SetBatchConfigurations.Handle(context);
                await Steps.SetCustomerDetails.Handle(context);

                return new Result
                {
                    ParseTransactions = request.ParseTransactions
                };
            }
        }
    }
}
