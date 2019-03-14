using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Commit
{
    public partial class Commit
    {
        public class Command : IRequest<Result>
        {
            public List<ParseModel> ParseTransactions { get; set; }

            public Guid ProcessId { get; set; }
        }

        public class Result
        {
            public List<ParseModel> ParseModel { get; set; }

            public List<BatchDetails> BatchDetailsList { get; set; }

            public List<int> TransactionIdsForRollback { get; set; }

            public bool AreTransactionsCommited { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IHttpClientFactory _httpClient;

            public Handler(IHttpClientFactory httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = await BillingRequest(request);

                return result;
            }

            private async Task<Result> BillingRequest(Command command)
            {
                var result = new Result
                {
                    AreTransactionsCommited = true
                };
                var httpResponse = new HttpResponseMessage();

                try
                {
                    var httpClient = _httpClient.CreateClient("Billing");
                    var jsonRequest = JsonConvert.SerializeObject(command.ParseTransactions);
                    var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    httpResponse = await httpClient.PostAsync("deliverytrans/cfn", httpContent);
                    httpResponse.EnsureSuccessStatusCode();

                    var response = await httpResponse.Content.ReadAsStringAsync();
                    var billingResult = JsonConvert.DeserializeObject<BillingResponse>(response);
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

                    result.AreTransactionsCommited = false;
                }

                return result;
            }
        }
    }
}
