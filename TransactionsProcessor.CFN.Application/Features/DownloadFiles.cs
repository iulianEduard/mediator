using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Features.FTP;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class DownloadFiles
    {
        public class Command : IRequest<Result>
        {
            public string ContentType { get; set; }
        }

        public class Result
        {
            public List<string> DownloadedFiles { get; set; }
        }

        public class DownloadFilesCommand : IRequestHandler<Command, Result>
        {
            private readonly IMediator _mediator;

            public DownloadFilesCommand(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = new Result();

                var downloadLocation = await _mediator.Send(new FTPDownloadLocation.Query(request.ContentType), cancellationToken);
                var ftpCredentials = await _mediator.Send(new FTPCredentials.Query(request.ContentType), cancellationToken);

                var downloadedFiles = new List<string>()
                {
                    @"C:\Temp\File1.csv"
                };

                await Task.Run(() =>
                 {
                     result = new Result
                     {
                         DownloadedFiles = downloadedFiles
                     };
                 });

                return result;
            }
        }
    }
}
