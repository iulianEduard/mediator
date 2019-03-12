using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class Export
    {
        public class Command : IRequest<Result>
        {
            public string ContentType { get; set; }

            public Dictionary<int, ParseModel> CfnFileDictionary { get; set; }
        }

        public class Result
        {

        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public Handler()
            {

            }

            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
