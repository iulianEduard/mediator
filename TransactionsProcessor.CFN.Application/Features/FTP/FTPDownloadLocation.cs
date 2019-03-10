using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.FTP
{
    public class FTPDownloadLocation
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
            public string DownloadLocation { get; set; }
        }

        public class FTPDownloadLocationQuery : IRequestHandler<Query, Result>
        {
            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var downloadLocation = await GetDownloadLocation(request);

                var result = new Result
                {
                    DownloadLocation = downloadLocation
                };

                return result;
            }

            private async Task<string> GetDownloadLocation(Query query)
            {
                var location = string.Empty;
                await Task.Run(() =>
                {
                    location = @"C:\Temp\CFN\";
                });

                return location;
            }
        }
    }
}
