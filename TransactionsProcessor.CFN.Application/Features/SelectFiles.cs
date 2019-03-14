using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Services.RemoteFileChecker;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class SelectFiles
    {
        public class Command : IRequest<Result>
        {
            public string ContentType { get; set; }

            public List<string> FilesToDownload { get; set; }
        }

        public class Result
        {
            public List<ResultDetail> FilesToBeProcessed { get; set; }
        }

        public class ResultDetail
        {
            public string FileName { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnDatabase _database;
            private readonly IImportedFileChecker _fileChecker;

            public Handler(ICfnDatabase database, IImportedFileChecker fileChecker)
            {
                _database = database;
                _fileChecker = fileChecker;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var ftpCredentials = await _database.QuerySingle<Credentials>("", new { request.ContentType });

                var remoteFileChecker = RemoteFileCheckerFactory.GetByName(ftpCredentials.TransferProtocol);

                var remoteFileRequest = new RemoteFileRequest
                {
                };

                var remoteFileCheckerCredentials = new RemoteFileCredentials
                {
                    Host = ftpCredentials.Host,
                    Directory = ftpCredentials.Directory,
                    Port = ftpCredentials.Port,
                    UserName = ftpCredentials.UserName,
                    UserPassword = ftpCredentials.UserPassword
                };

                var remoteFiles = await remoteFileChecker.GetRemoteFiles(remoteFileRequest);

                var filesToBeProcessed = new Result();

                var fileCheckResult = await _fileChecker.Check(remoteFiles.Select(rf => rf.FileName).ToList());

                foreach(var fileCheck in fileCheckResult)
                {
                    if (fileCheck.Imported)
                    {
                        filesToBeProcessed.FilesToBeProcessed.Add(new ResultDetail { FileName = fileCheck.File });
                    }
                }

                return filesToBeProcessed;
            }
        }

        public class Credentials
        {
            public string Host { get; set; }

            public int Port { get; set; }

            public string TransferProtocol { get; set; }

            public string Directory { get; set; }

            public string UserName { get; set; }

            public string UserPassword { get; set; }
        }
    }
}
