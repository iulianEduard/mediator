using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;

namespace TransactionsProcessor.CFN.Application.Features.SendToQC
{
    public partial class SendToQC
    {
        public class Command : IRequest<Result>
        {
            public Guid ProcessId { get; set; }
        }

        public class Result
        {
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnDatabase _database;
            private readonly IHttpClientFactory _httpClient;

            public Handler(ICfnDatabase database, IHttpClientFactory httpClient)
            {
                _database = database;
                _httpClient = httpClient;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var generatedBatches = await _database.Query<BatchInfo>("", new { request.ProcessId });

                await SendToQC(generatedBatches);

                return new Result();
            }

            private async Task SendToQC(IEnumerable<BatchInfo> batchInfos)
            {
                var httpClient = _httpClient.CreateClient("QCClient");
                var jsonRequest = JsonConvert.SerializeObject(batchInfos);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("qcclient/multiMessage", httpContent);

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch(HttpRequestException)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError:
                            break;
                        case HttpStatusCode.BadRequest:
                            break;
                        case HttpStatusCode.NotFound:
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
