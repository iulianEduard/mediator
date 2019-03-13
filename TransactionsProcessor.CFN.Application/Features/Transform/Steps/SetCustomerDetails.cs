using System.Linq;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.Transform.Steps
{
    public static class SetCustomerDetails
    {
        public static async Task Handle(Transform.Context context)
        {
            var customerCards = await context.Database.Query<Transform.Customer>("cfn.usp_GetCustomerCards");
            var customerMissingIds = await context.Database.QuerySingle<Transform.CustomerType>("cfn.usp_GetSystemStatusForCFNCustomers", new { CFNForeign = "CFNForeignCustomer", CFNUnknown = "CFNUnknownCustomer" });

            foreach (var billingTransaction in context.BillngTransactions)
            {
                var customerId = customerCards.Where(cc => cc.CardId == billingTransaction.CardId).Select(cc => cc.CustomerId).FirstOrDefault();

                if(billingTransaction.TransactionType == "F")
                {
                    billingTransaction.CustomerId = customerMissingIds.ForeignCustomerId;
                    continue;
                }

                billingTransaction.CustomerId = customerId == 0 
                    ? customerMissingIds.UnknownCustomerId 
                    : customerId;
            }
        }
    }
}
