using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.FTP
{
    public class FTPDownload
    {
        public class Command : IRequest<Result>
        {
            public string IP { get; set; }

            public string Location { get; set; }

            public string UserName { get; set; }

            public string UserPassword { get; set; }

            public string TransferProtocol { get; set; }
        }

        public class Result
        {
            public List<string> DownloadedFiles { get; set; }
        }

        public class FTPDownloadCommand : IRequestHandler<Command, Result>
        {
            public Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
