using System;
using System.Collections.Generic;
using System.Linq;
using TransactionsProcessor.CFN.Application.Core.Extensions;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Features.Transform.Steps
{
    public static class Prepare
    {
        public static void Handle(Transform.Context context)
        {
            var billingModel = new List<BillingModel>();
            var records = context.ParseTransactions;

            context.BillngTransactions = records.Select(recordItem => new BillingModel
            {
                DeliveryId = 0,
                BatchNumber = recordItem.SiteIdNumber,
                Gallons = recordItem.Quantity,
                ProductCode = recordItem.ProductCode,
                TicketNumber = recordItem.TransactionNumber,
                DeliveryDate = recordItem.ToDeliveryDate(),
                DeliveryEndTime = recordItem.ToDeliveryEndTime(),
                PONumber = recordItem.ToPONumber(),
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
                },
                FleetTransList = new List<FleetTrans>
                {
                    new FleetTrans
                    {
                        FleetId = 0,
                        DeliveryId = 0,
                        Asset = recordItem.ToAsset(),
                        AssetData = recordItem.Odometer,
                        Gallons = recordItem.Quantity
                    }
                },
            }).ToList();
        }
    }

    public static class PrepareExtensions
    {
        public static DateTime ToDeliveryDate(this ParseModel record)
        {
            return record.DateCompleted.ToDateTime();
        }

        public static DateTime ToDeliveryEndTime(this ParseModel record)
        {
            var dateCompleted = record.ToDeliveryDate();
            return record.TimeCompleted.ToDateTime(dateCompleted);
        }

        public static string ToPONumber(this ParseModel record)
        {
            return string.IsNullOrEmpty(record.POInvoiceNumber) ? null : record.POInvoiceNumber;
        }

        public static string ToAsset(this ParseModel record)
        {
            return string.IsNullOrWhiteSpace(record.VehicleIdentifier) ? "Truck" : record.VehicleIdentifier;
        }
    }
}
