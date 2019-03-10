using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Services.FTPServices;

namespace TransactionsProcessor.CFN.Application.Features.FTP
{
    public class FTPDownload
    {
        public class Command : IRequest<Result>
        {
            public Request Request { get; set; }

            public Command(Request request)
            {
                Request = request;
            }
        }

        public class Request
        {
            public string IP { get; set; }

            public string Location { get; set; }

            public string UserName { get; set; }

            public string UserPassword { get; set; }

            public string TransferProtocol { get; set; }

            public string DownloadLocation { get; set; }
        }

        public class Result
        {
            public List<string> DownloadedFiles { get; set; }
        }

        public class FTPDownloadCommand : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                return await DownloadFilesAsync(request);
            }

            private async Task<Result> DownloadFilesAsync(Command command)
            {
                ISSHService sshService;

                var request = command.Request;

                if (request.TransferProtocol.ToLower() == "ftp")
                {
                    sshService = new FTPService();
                }
                else if (request.TransferProtocol.ToLower() == "ssl")
                {
                    sshService = new SSLService();
                }
                else if (request.TransferProtocol.ToLower() == "sftp")
                {
                    sshService = new SFTPService();
                }
                else
                {
                    throw new System.Exception("Wrong SSH configuration!");
                }

                var downloadRequest = new DownloadRequest
                {
                    Options = new FTPOptions
                    {
                        IP = request.IP,
                        Location = request.Location,
                        TransferProtocol = request.TransferProtocol,
                        UserName = request.UserName,
                        UserPassword = request.UserPassword
                    },
                    DownloadLocation = request.DownloadLocation
                };

                var ftpDownloadResponse = await sshService.DownloadFilesAsync(downloadRequest);

                var result = new Result();
                ftpDownloadResponse.ForEach(f => result.DownloadedFiles.Add(f.FileName));

                return result;
            }
        }
    }
}
