using MediatR;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;

namespace TransactionsProcessor.CFN.Application.Features.Rollback
{
    public class Rollback
    {
        public class Command : IRequest<Result>
        {
            public int FileId { get; set; }

            public string Message { get; set; }

            public bool TransactionsAreCommited { get; set; }

            public Guid ProcessId { get; set; }
        }

        public class Result
        { }

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
                await _database.QuerySingle<int>("", new { request.FileId, request.Message });

                await RollbackBilingTransactions(request);

                return new Result();
            }

            private async Task RollbackBilingTransactions(Command command)
            {
                if(command.TransactionsAreCommited)
                {
                    var httpClient = _httpClient.CreateClient("Billing");
                    var jsonRequest = JsonConvert.SerializeObject(command.ProcessId);
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    var httpResponse = await httpClient.PostAsync("deliverytrans/cfn/rollback", httpContent);

                    try
                    {
                        httpResponse.EnsureSuccessStatusCode();

                        var response = await httpResponse.Content.ReadAsStringAsync();
                    }
                    catch (HttpRequestException)
                    {
                        switch (httpResponse.StatusCode)
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
}
