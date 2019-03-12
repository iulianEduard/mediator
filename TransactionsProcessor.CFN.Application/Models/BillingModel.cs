using System;
using System.Collections.Generic;

namespace TransactionsProcessor.CFN.Application.Models
{
    public class BillingModel
    {
        public int DeliveryId { get; set; }

        public int BatchNumber { get; set; }

        public int CustomerId { get; set; }

        public decimal Gallons { get; set; }

        public int ProductCode { get; set; }

        public long TicketNumber { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DateTime DeliveryEndTime { get; set; }

        public string PONumber { get; set; }

        public int TruckId { get; set; }

        public int DriverId { get; set; }

        public int BatchCompanyId { get; set; }

        public List<FleetTrans> FleetTransList { get; set; }

        public DeliveryTransPricing DeliveryTransPricingItem { get; set; }

        //[IgnoreMe]
        public int SiteId { get; set; }

        //[IgnoreMe]
        public string CardId { get; set; }

        //[IgnoreMe]
        public string NameOnCard { get; set; }

        //[IgnoreMe]
        public string TransactionType { get; set; }
    }

    public class FleetTrans
    {
        public int FleetId { get; set; }

        public int DeliveryId { get; set; }

        public decimal Gallons { get; set; }

        public int AssetData { get; set; }

        public string Asset { get; set; }
    }

    public class DeliveryTransPricing
    {
        public int Id { get; set; }

        public int DeliveryId { get; set; }

        public decimal BrokerCost { get; set; }

        public decimal BrokerFee { get; set; }

        public decimal InvoicePrice { get; set; }

        public decimal OriginalPrice { get; set; }
    }
}
