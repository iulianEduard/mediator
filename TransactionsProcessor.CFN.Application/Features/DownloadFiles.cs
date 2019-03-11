using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Services.Downloader;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class DownloadFiles
    {
        public class Command : IRequest<Result>
        {
            public string ContentType { get; set; }

            public string FileToDownload { get; set; }
        }

        public class Result
        {
            public Dictionary<int, string> DownloadedFiles { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnDatabase _database;
            private readonly IApplicationFilesService _applicationFilesService;

            public Handler(ICfnDatabase database, IApplicationFilesService applicationFilesService)
            {
                _database = database;
                _applicationFilesService = applicationFilesService;
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
                        Host = ftpCredentials.Host,
                        Port = ftpCredentials.Port,
                        Directory = ftpCredentials.Directory,
                        UserName = ftpCredentials.UserName,
                        UserPassword = ftpCredentials.UserPassword
                    },
                    FilesToDownload = new List<string> { request.FileToDownload }
                };

                var downloader = DownloaderFactory.GetByName(ftpCredentials.TransferProtocol);
                var downloadResponse = await downloader.DownloadFilesAsync(downloadRequest);

                var result = new Result
                {
                    DownloadedFiles = new Dictionary<int, string>()
                };

                foreach(var file in downloadResponse.Details)
                {
                    var applicationFilesId = await _applicationFilesService.Insert(file.FileName, "CFN");

                    result.DownloadedFiles.Add(applicationFilesId, file.FileName);
                }

                return result;
            }
        }

        public class DownloadSettings
        {
            public string Host { get; set; }

            public int Port { get; set; }

            public string Directory { get; set; }

            public string UserName { get; set; }

            public string UserPassword { get; set; }

            public string TransferProtocol { get; set; }
        }
    }
}
