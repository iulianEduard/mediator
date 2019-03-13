using System.Linq;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Features.Transform.Steps
{
    public static class SetBatchConfigurations
    {
        public static async Task Handle(Transform.Context context)
        {
            var batchConfigurations = await context.Database.Query<Transform.BatchConfiguration>("cfn.usp_GetSystemStatusForCFNCustomers");

            foreach (var batchConfiguration in batchConfigurations)
            {
                //
                // Update transactions with sites that are in the system and are diesel owned
                //
                var groupedDeliveriesBySite = context.BillngTransactions.Where(cfn => cfn.SiteId == batchConfiguration.SiteId && batchConfiguration.DieselOwned == true).ToList();

                foreach (var groupedDelivery in groupedDeliveriesBySite)
                {
                    groupedDelivery.TruckId = batchConfiguration.TruckId;
                    groupedDelivery.DriverId = batchConfiguration.DriverId;
                    groupedDelivery.BatchCompanyId = batchConfiguration.BatchCompanyId;
                }
            }

            //
            // Update transactions with sites that are not in the system
            //
            var unknownSitesInFile = context.BillngTransactions.Where(cfn => !batchConfigurations.Any(bc => bc.SiteId == cfn.SiteId));
            var foreignBatch = batchConfigurations.Where(bc => bc.SiteId == 0).FirstOrDefault();

            foreach (var unknownSite in unknownSitesInFile)
            {
                unknownSite.TruckId = foreignBatch.TruckId;
                unknownSite.DriverId = foreignBatch.DriverId;
                unknownSite.BatchCompanyId = foreignBatch.BatchCompanyId;
                unknownSite.BatchNumber = Transform.Constants.InvalidBatchNumber;
            }

            //
            // Update transactions with sites that are in the system but are not diesel owned
            //
            var notDieselSitesInFile = context.BillngTransactions.Where(cfn => batchConfigurations.Any(bc => bc.SiteId == cfn.SiteId && (bc.DieselOwned == false || bc.DieselOwned == null)));

            foreach (var notDieselSite in notDieselSitesInFile)
            {
                notDieselSite.TruckId = foreignBatch.TruckId;
                notDieselSite.DriverId = foreignBatch.DriverId;
                notDieselSite.BatchCompanyId = foreignBatch.BatchCompanyId;
                notDieselSite.BatchNumber = Transform.Constants.InvalidBatchNumber;
            }
        }
    }
}
