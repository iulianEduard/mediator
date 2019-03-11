using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Core;
using TransactionsProcessor.CFN.Application.Helpers;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features
{
    public class PrepareTransactions
    {
        public class Command : IRequest<Result>
        {
            public Dictionary<int, CfnFileModel> CfnRecordsDictonary { get; set; }
        }

        public class Result
        {
            public Dictionary<int, CfnFileModel> CfnRecordsDictionary { get; set; }

            public Dictionary<int, CfnBillingModel> CfnBillingRecordsDictonary { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly ICfnDatabase _cfnDatabase;

            public Handler(ICfnDatabase cfnDatabase)
            {
                _cfnDatabase = cfnDatabase;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                await SetCustomerCardNames(request);

                var billingDictionary = PrepareData(request.CfnRecordsDictonary);

                await SetDDFuelTypes(billingDictionary);

                await SetBatchConfiguration(billingDictionary);

                await SetCustomerDetails(billingDictionary);

                return new Result
                {
                    CfnRecordsDictionary = request.CfnRecordsDictonary,
                    CfnBillingRecordsDictonary = billingDictionary
                };
            }

            private async Task SetCustomerCardNames(Command command)
            {
                var customerCards = await _cfnDatabase.Query<CFNCustomer>("cfn.usp_GetCustomerCards");

                foreach (var cfnRecord in command.CfnRecordsDictonary.Values)
                {
                    var nameOnCard = customerCards.Where(cc => cc.CardId == cfnRecord.CardId.ToString()).Select(cc => cc.NameOnCard).FirstOrDefault();
                    cfnRecord.NameOnCard = string.IsNullOrWhiteSpace(nameOnCard) ? "Not found" : nameOnCard;
                }
            }

            private Dictionary<int, CfnBillingModel> PrepareData(Dictionary<int, CfnFileModel> cfnRecords)
            {
                var billingDictionary = new Dictionary<int, CfnBillingModel>();

                foreach (var cfnRecordPair in cfnRecords)
                {
                    var cfnRecord = cfnRecordPair.Value;

                    var fleetTransList = new List<FleetTrans>
                {
                    new FleetTrans
                    {
                        FleetId = 0,
                        DeliveryId = 0,
                        Asset = string.IsNullOrWhiteSpace(cfnRecord.VehicleIdentifier) ? "Truck" : cfnRecord.VehicleIdentifier,
                        AssetData = cfnRecord.Odometer,
                        Gallons = cfnRecord.Quantity
                    }
                };

                    var deliveryDate = cfnRecord.DateCompleted.ToDateTime();

                    var deliveryTransItem = new CfnBillingModel
                    {
                        DeliveryId = 0,
                        BatchNumber = cfnRecord.SiteIdNumber,
                        Gallons = cfnRecord.Quantity,
                        ProductCode = cfnRecord.ProductCode,
                        TicketNumber = cfnRecord.TransactionNumber,
                        DeliveryDate = deliveryDate,
                        DeliveryEndTime = cfnRecord.TimeCompleted.ToDateTime(deliveryDate),
                        PONumber = string.IsNullOrEmpty(cfnRecord.POInvoiceNumber) ? null : cfnRecord.POInvoiceNumber,
                        FleetTransList = fleetTransList,
                        SiteId = cfnRecord.SiteIdNumber,
                        CardId = cfnRecord.CardId.ToString(),
                        TransactionType = cfnRecord.TransactionLocationIndicator,
                        DeliveryTransPricingItem = new DeliveryTransPricing
                        {
                            Id = 0,
                            DeliveryId = 0,
                            BrokerCost = cfnRecord.PumpPrice,
                            BrokerFee = cfnRecord.HaulRate,
                            InvoicePrice = cfnRecord.Price,
                            OriginalPrice = cfnRecord.CFNPrice
                        }
                    };

                    billingDictionary.Add(cfnRecordPair.Key, deliveryTransItem);
                }

                return billingDictionary;
            }

            private async Task SetDDFuelTypes(Dictionary<int, CfnBillingModel> cfnBillingDictionary)
            {
                var cfnFuelTypes = await _cfnDatabase.Query<CFNFuelType>("cfn.usp_GetFuelTypes");

                foreach (var deliveryTrans in cfnBillingDictionary.Values)
                {
                    var fuelAssociation = cfnFuelTypes.Where(cfn => cfn.CFNProductCode == deliveryTrans.ProductCode).FirstOrDefault();

                    if (fuelAssociation == null)
                    {
                        fuelAssociation = new CFNFuelType { FuelTypeId = 100 };
                    }

                    deliveryTrans.ProductCode = fuelAssociation.FuelTypeId;
                }
            }

            private async Task SetBatchConfiguration(Dictionary<int, CfnBillingModel> cfnBillingDictionary)
            {
                var batchConfigurations = await _cfnDatabase.Query<CFNBatchConfiguration>("cfn.usp_GetSystemStatusForCFNCustomers");

                foreach (var batchConfiguration in batchConfigurations)
                {
                    var groupedDeliveriesBySite = cfnBillingDictionary.Values.Where(cfn => cfn.SiteId == batchConfiguration.SiteId && batchConfiguration.DieselOwned == true).ToList();

                    foreach (var groupedDelivery in groupedDeliveriesBySite)
                    {
                        groupedDelivery.TruckId = batchConfiguration.TruckId;
                        groupedDelivery.DriverId = batchConfiguration.DriverId;
                        groupedDelivery.BatchCompanyId = batchConfiguration.BatchCompanyId;
                    }
                }

                var unknownSitesInFile = cfnBillingDictionary.Values.Where(cfn => !batchConfigurations.Any(bc => bc.SiteId == cfn.SiteId));
                var foreignBatch = batchConfigurations.Where(bc => bc.SiteId == 0).FirstOrDefault();

                foreach (var unknownSite in unknownSitesInFile)
                {
                    unknownSite.TruckId = foreignBatch.TruckId;
                    unknownSite.DriverId = foreignBatch.DriverId;
                    unknownSite.BatchCompanyId = foreignBatch.BatchCompanyId;
                    unknownSite.BatchNumber = -666;
                }

                var notDieselSitesInFile = cfnBillingDictionary.Values.Where(cfn => batchConfigurations.Any(bc => bc.SiteId == cfn.SiteId && (bc.DieselOwned == false || bc.DieselOwned == null)));

                foreach (var notDieselSite in notDieselSitesInFile)
                {
                    notDieselSite.TruckId = foreignBatch.TruckId;
                    notDieselSite.DriverId = foreignBatch.DriverId;
                    notDieselSite.BatchCompanyId = foreignBatch.BatchCompanyId;
                    notDieselSite.BatchNumber = -666;
                }
            }

            private async Task SetCustomerDetails(Dictionary<int, CfnBillingModel> cfnBillingDictionary)
            {
                var customerCards = await _cfnDatabase.Query<CFNCustomer>("cfn.usp_GetCustomerCards");
                var customerMissingIds = await _cfnDatabase.QuerySingle<CFNCustomerType>("cfn.usp_GetSystemStatusForCFNCustomers", new { CFNForeign = "CFNForeignCustomer", CFNUnknown = "CFNUnknownCustomer" });

                foreach (var deliveryTransCFN in cfnBillingDictionary.Values)
                {
                    var customerId = customerCards.Where(cc => cc.CardId == deliveryTransCFN.CardId).Select(cc => cc.CustomerId).FirstOrDefault();

                    switch (deliveryTransCFN.TransactionType)
                    {
                        case "F":
                            deliveryTransCFN.CustomerId = customerMissingIds.ForeignCustomerId;
                            break;
                        default:
                            if (customerId == 0)
                            {
                                deliveryTransCFN.CustomerId = customerMissingIds.UnknownCustomerId;
                            }
                            else
                            {
                                deliveryTransCFN.CustomerId = customerId;
                            }
                            break;

                    }
                }
            }
        }

        public class CFNCustomer
        {
            public string CardId { get; set; }

            public int CustomerId { get; set; }

            public string NameOnCard { get; set; }
        }

        public class CFNCustomerType
        {
            public int ForeignCustomerId { get; set; }

            public int UnknownCustomerId { get; set; }
        }

        public class CFNFuelType
        {
            public int Id { get; set; }

            public int CFNProductCode { get; set; }

            public int FuelTypeId { get; set; }
        }

        public class CFNBatchConfiguration
        {
            public int Id { get; set; }

            public int SiteId { get; set; }

            public int TruckId { get; set; }

            public int DriverId { get; set; }

            public int BatchCompanyId { get; set; }

            public bool? DieselOwned { get; set; }
        }
    }
}
