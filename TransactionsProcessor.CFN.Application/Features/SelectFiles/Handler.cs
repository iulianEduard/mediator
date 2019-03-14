using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Services.RemoteFileChecker;

namespace TransactionsProcessor.CFN.Application.Features.SelectFiles
{
    public partial class SelectFiles
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

            public Handler(ICfnDatabase database)
            {
                _database = database;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var ftpCredentials = await _database.QuerySingle<Credentials>("", new { request.ContentType });
                var remoteFileChecker = RemoteFileCheckerFactory.GetByName(ftpCredentials.TransferProtocol);

                var remoteFileRequest = new RemoteFileRequest
                {
                    Credentials = new RemoteFileCredentials
                    {
                        Host = ftpCredentials.Host,
                        Directory = ftpCredentials.Directory,
                        Port = ftpCredentials.Port,
                        UserName = ftpCredentials.UserName,
                        UserPassword = ftpCredentials.UserPassword
                    },
                    Options = new RemoteFileOptions
                    {
                        ExtensionsToDownload = new List<string> { ".csv" }
                    }
                };

                var remoteFiles = await remoteFileChecker.GetRemoteFiles(remoteFileRequest);
                var filesToProcess = await Check(remoteFiles.Select(rf => rf.FileName).ToList());

                var filesToBeProcessed = new Result
                {
                    FilesToBeProcessed = SelectFilesFromFtp(filesToProcess)
                };

                return filesToBeProcessed;
            }

            private async Task<IEnumerable<FileCheck>> Check(IEnumerable<string> filesFromFtp)
            {
                return await _database.Query<FileCheck>("", new { filesFromFtp });
            }

            private List<ResultDetail> SelectFilesFromFtp(IEnumerable<FileCheck> fileFromFtp)
            {
                var selectFiles = new List<ResultDetail>();

                foreach (var fileCheck in fileFromFtp)
                {
                    if (fileCheck.Imported)
                    {
                        selectFiles.Add(new ResultDetail { FileName = fileCheck.File });
                    }
                }

                return selectFiles;
            }
        }        
    }
}
