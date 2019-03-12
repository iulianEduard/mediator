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
    public class Transform
    {
        public class Command : IRequest<Result>
        {
            public List<ParseModel> ParseTransactions { get; set; }
        }

        public class Result
        {
            public List<ParseModel> ParseTransactions { get; set; }

            public List<BillingModel> BillingTransactions { get; set; }
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

                var billingRequestDto = PrepareData(request);

                await SetDDFuelTypes(billingRequestDto);

                await SetBatchConfiguration(billingRequestDto);

                await SetCustomerDetails(billingRequestDto);

                return new Result
                {
                    ParseTransactions = request.ParseTransactions,
                    BillingTransactions = billingRequestDto
                };
            }

            private async Task SetCustomerCardNames(Command command)
            {
                var customerCards = await _cfnDatabase.Query<CFNCustomer>("cfn.usp_GetCustomerCards");

                foreach (var cfnRecord in command.ParseTransactions)
                {
                    var nameOnCard = customerCards.Where(cc => cc.CardId == cfnRecord.CardId.ToString()).Select(cc => cc.NameOnCard).FirstOrDefault();
                    cfnRecord.NameOnCard = string.IsNullOrWhiteSpace(nameOnCard) ? "Not found" : nameOnCard;
                }
            }

            private List<BillingModel> PrepareData(Command command)
            {
                var billingModel = new List<BillingModel>();
                var records = command.ParseTransactions;

                foreach (var recordItem in records)
                {
                    var fleetTransList = new List<FleetTrans>
                    {
                        new FleetTrans
                        {
                            FleetId = 0,
                            DeliveryId = 0,
                            Asset = string.IsNullOrWhiteSpace(recordItem.VehicleIdentifier) ? "Truck" : recordItem.VehicleIdentifier,
                            AssetData = recordItem.Odometer,
                            Gallons = recordItem.Quantity
                        }
                    };

                    var deliveryDate = recordItem.DateCompleted.ToDateTime();

                    var deliveryTransItem = new BillingModel
                    {
                        DeliveryId = 0,
                        BatchNumber = recordItem.SiteIdNumber,
                        Gallons = recordItem.Quantity,
                        ProductCode = recordItem.ProductCode,
                        TicketNumber = recordItem.TransactionNumber,
                        DeliveryDate = deliveryDate,
                        DeliveryEndTime = recordItem.TimeCompleted.ToDateTime(deliveryDate),
                        PONumber = string.IsNullOrEmpty(recordItem.POInvoiceNumber) ? null : recordItem.POInvoiceNumber,
                        FleetTransList = fleetTransList,
                        SiteId = recordItem.SiteIdNumber,
                        CardId = recordItem.CardId.ToString(),
                        TransactionType = recordItem.TransactionLocationIndicator,
                        DeliveryTransPricingItem = new DeliveryTransPricing
                        {
                            Id = 0,
                            DeliveryId = 0,
                            BrokerCost = recordItem.PumpPrice,
                            BrokerFee = recordItem.HaulRate,
                            InvoicePrice = recordItem.Price,
                            OriginalPrice = recordItem.CFNPrice
                        }
                    };

                    billingModel.Add(deliveryTransItem);
                }

                return billingModel;
            }

            private async Task SetDDFuelTypes(List<BillingModel> billingRequestModel)
            {
                var cfnFuelTypes = await _cfnDatabase.Query<CFNFuelType>("cfn.usp_GetFuelTypes");

                foreach (var billingItem in billingRequestModel)
                {
                    var fuelAssociation = cfnFuelTypes.Where(cfn => cfn.CFNProductCode == billingItem.ProductCode).FirstOrDefault();

                    if (fuelAssociation == null)
                    {
                        fuelAssociation = new CFNFuelType { FuelTypeId = 100 };
                    }

                    billingItem.ProductCode = fuelAssociation.FuelTypeId;
                }
            }

            private async Task SetBatchConfiguration(List<BillingModel> billingRequestModel)
            {
                var batchConfigurations = await _cfnDatabase.Query<CFNBatchConfiguration>("cfn.usp_GetSystemStatusForCFNCustomers");

                foreach (var batchConfiguration in batchConfigurations)
                {
                    var groupedDeliveriesBySite = billingRequestModel.Where(cfn => cfn.SiteId == batchConfiguration.SiteId && batchConfiguration.DieselOwned == true).ToList();

                    foreach (var groupedDelivery in groupedDeliveriesBySite)
                    {
                        groupedDelivery.TruckId = batchConfiguration.TruckId;
                        groupedDelivery.DriverId = batchConfiguration.DriverId;
                        groupedDelivery.BatchCompanyId = batchConfiguration.BatchCompanyId;
                    }
                }

                var unknownSitesInFile = billingRequestModel.Where(cfn => !batchConfigurations.Any(bc => bc.SiteId == cfn.SiteId));
                var foreignBatch = batchConfigurations.Where(bc => bc.SiteId == 0).FirstOrDefault();

                foreach (var unknownSite in unknownSitesInFile)
                {
                    unknownSite.TruckId = foreignBatch.TruckId;
                    unknownSite.DriverId = foreignBatch.DriverId;
                    unknownSite.BatchCompanyId = foreignBatch.BatchCompanyId;
                    unknownSite.BatchNumber = -666;
                }

                var notDieselSitesInFile = billingRequestModel.Where(cfn => batchConfigurations.Any(bc => bc.SiteId == cfn.SiteId && (bc.DieselOwned == false || bc.DieselOwned == null)));

                foreach (var notDieselSite in notDieselSitesInFile)
                {
                    notDieselSite.TruckId = foreignBatch.TruckId;
                    notDieselSite.DriverId = foreignBatch.DriverId;
                    notDieselSite.BatchCompanyId = foreignBatch.BatchCompanyId;
                    notDieselSite.BatchNumber = -666;
                }
            }

            private async Task SetCustomerDetails(List<BillingModel> billingRequestModel)
            {
                var customerCards = await _cfnDatabase.Query<CFNCustomer>("cfn.usp_GetCustomerCards");
                var customerMissingIds = await _cfnDatabase.QuerySingle<CFNCustomerType>("cfn.usp_GetSystemStatusForCFNCustomers", new { CFNForeign = "CFNForeignCustomer", CFNUnknown = "CFNUnknownCustomer" });

                foreach (var deliveryTransCFN in billingRequestModel)
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
