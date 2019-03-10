using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.FTP
{
    public class FTPCredentials
    {
        public class Query : IRequest<Result>
        {
            public string ContentType { get; }

            public Query(string contentType)
            {
                ContentType = contentType;
            }
        }

        public class Result
        {
            public string IP { get; set; }

            public string Location { get; set; }

            public string UserName { get; set; }

            public string UserPassword { get; set; }

            public string TransferProtocol { get; set; }
        }

        public class FTPCredentialssQuery : IRequestHandler<Query, Result>
        {
            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                return await GetFTPCredentials(request);
            }

            private async Task<Result> GetFTPCredentials(Query query)
            {
                var result = new Result();

                await Task.Run(() =>
                 {
                     result = new Result
                     {
                         IP = "192.168.10.10",
                         Location = @"\CFN",
                         TransferProtocol = "SSL",
                         UserName = "edv",
                         UserPassword = "edv2012"
                     };
                 });

                return result;
            }
        }
    }
}
