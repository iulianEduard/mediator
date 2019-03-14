using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;

namespace TransactionsProcessor.CFN.Application.Features.SelectFailedFiles
{
    public partial class SelectFailedFiles
    {
        public class Command : IRequest<Result>
        {
            public string ContentType { get; set; }
        }

        public class Result
        {

        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IAfDatabase _database;

            public Handler(IAfDatabase database)
            {
                _database = database;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var filesToBeProcessed = await _database.Query<FilesToBeProcessed>("", new { request.ContentType });

                throw new NotImplementedException();
            }
        }

    }
}
