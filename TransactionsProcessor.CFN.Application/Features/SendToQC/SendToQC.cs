using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.SendToQC
{
    public partial class SendToQC
    {
        public class Command : IRequest<Result>
        {
            public List<SendToQC.BatchInfo> BatchInfo { get; set; }
        }

        public class Result
        {
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
