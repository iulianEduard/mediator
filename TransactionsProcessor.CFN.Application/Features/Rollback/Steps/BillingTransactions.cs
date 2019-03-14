using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.Rollback.Steps
{
    //TODO: need to decide if this is a step!!!
    public class BillingTransactions
    {
        private readonly IHttpClientFactory _httpClient;

        public BillingTransactions(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Rollback(List<int> billingTransactionsIds)
        {
            var httpClient = _httpClient.CreateClient("Billing");
            var jsonRequest = JsonConvert.SerializeObject(billingTransactionsIds);
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
