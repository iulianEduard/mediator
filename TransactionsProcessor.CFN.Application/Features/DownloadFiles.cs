using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Services.Downloader;

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
            public IEnumerable<string> DownloadedFiles { get; set; }
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
                var downloadLocation = await _database.QuerySingle<string>("", new { request.ContentType }); 
                var ftpCredentials = await _database.QuerySingle<DownloadSettings>("", new { request.ContentType });

                var downloadRequest = new DownloadRequest
                {
                    DownloadLocation = downloadLocation,
                    Options = new Services.Downloader.DownloadSettings
                    {
                        Host = ftpCredentials.IP,
                        Port = ftpCredentials.Port,
                        Directory = ftpCredentials.Location,
                        TransferProtocol = ftpCredentials.TransferProtocol,
                        UserName = ftpCredentials.UserName,
                        UserPassword = ftpCredentials.UserPassword
                    }
                };

                var downloader = DownloaderFactory.GetByName(ftpCredentials.TransferProtocol);

                var response = await downloader.DownloadFilesAsync(downloadRequest);

                return new Result
                {
                    DownloadedFiles = response.Details.Select(r => r.FileName)
                };
            }
        }

        public class DownloadSettings
        {
            public string IP { get; set; }

            public int Port { get; set; }

            public string Location { get; set; }

            public string UserName { get; set; }

            public string UserPassword { get; set; }

            public string TransferProtocol { get; set; }
        }
    }
}
