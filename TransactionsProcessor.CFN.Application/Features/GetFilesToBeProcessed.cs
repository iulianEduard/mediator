using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Services.RemoteFileChecker;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class GetFilesToBeProcessed
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
            private readonly ICfnDatabase _cfnDatabase;
            private readonly IApplicationFilesService _applicationFilesService;

            public Handler(ICfnDatabase cfnDatabase, IApplicationFilesService applicationFilesService)
            {
                _cfnDatabase = cfnDatabase;
                _applicationFilesService = applicationFilesService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var downloadLocation = await _cfnDatabase.QuerySingle<string>("", new { request.ContentType });
                var ftpCredentials = await _cfnDatabase.QuerySingle<Credentials>("", new { request.ContentType });

                var remoteFileChecker = RemoteFileCheckerFactory.GetByName(ftpCredentials.TransferProtocol);

                var remoteFileCheckerCredentials = new RemoteFileCheckerCredentials
                {
                    Host = ftpCredentials.Host,
                    Directory = ftpCredentials.Directory,
                    Port = ftpCredentials.Port,
                    UserName = ftpCredentials.UserName,
                    UserPassword = ftpCredentials.UserPassword
                };

                var remoteFiles = await remoteFileChecker.GetRemoteFiles(remoteFileCheckerCredentials);

                var filesToBeProcessed = new Result();

                foreach(var remoteFile in remoteFiles)
                {
                    if(!await _applicationFilesService.CheckIfFileExists(remoteFile.FileName))
                    {
                        filesToBeProcessed.FilesToBeProcessed.Add(new ResultDetail { FileName = remoteFile.FileName });
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
