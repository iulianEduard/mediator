using System.Linq;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.Transform.Steps
{
    public static class ChangeFuelTypes
    {
        public static async Task Handle(Transform.Context context)
        {
            var cfnFuelTypes = await context.Database.Query<Transform.FuelType>("cfn.usp_GetFuelTypes");

            foreach (var billingItem in context.BillngTransactions)
            {
                var fuelAssociation = cfnFuelTypes.Where(cfn => cfn.CFNProductCode == billingItem.ProductCode).FirstOrDefault();

                if (fuelAssociation == null)
                {
                    fuelAssociation = new Transform.FuelType { FuelTypeId = 100 };
                }

                billingItem.ProductCode = fuelAssociation.FuelTypeId;
            }
        }
    }
}
