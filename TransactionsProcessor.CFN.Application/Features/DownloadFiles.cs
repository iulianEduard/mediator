using MediatR;
using System;
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
            public int FileId { get; set; }

            public string FileName { get; set; }

            public Guid ProcessId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnDatabase _database;
            private readonly IFileAdder _fileAdder;

            public Handler(ICfnDatabase database, IFileAdder fileAdder)
            {
                _database = database;
                _fileAdder = fileAdder;
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
                var downloadedFile = downloadResponse.Details.FirstOrDefault();

                var result = new Result
                {
                    ProcessId = Guid.NewGuid(),
                    FileId = await _fileAdder.AddFile(downloadedFile.FileName, "CFN"),
                    FileName = downloadedFile.FileName
                };

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
