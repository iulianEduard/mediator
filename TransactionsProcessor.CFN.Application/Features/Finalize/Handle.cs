using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles;
using TransactionsProcessor.CFN.Application.Core;

namespace TransactionsProcessor.CFN.Application.Features.Finalize
{
    public partial class Finalize
    {
        public class Command : IRequest<Result>
        {
            public Guid ProcessId { get; set; }

            public int FileId { get; set; }
        }

        public class Result
        {

        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IAfDatabase _afDatabase;
            private readonly ICfnDatabase _cfnDatabase;

            public Handler(IAfDatabase afDatabase, ICfnDatabase cfnDatabase)
            {
                _afDatabase = afDatabase;
                _cfnDatabase = cfnDatabase;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {


                throw new NotImplementedException();
            }
        }
    }
}
