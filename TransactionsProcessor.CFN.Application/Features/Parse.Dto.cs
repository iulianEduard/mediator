using FileHelpers;
using System;

namespace TransactionsProcessor.CFN.Application.Features
{
    public partial class Parse
    {
        [DelimitedRecord(",")]
        [IgnoreEmptyLines]
        public class Template
        {
            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int SiteIdNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int SequenceNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int StatusCode { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal TotalAmount { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int TransactionType { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int ProductCode { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string ProductType { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string ProductDescription { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal Price { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal Quantity { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int Odometer { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int PumpNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public long TransactionNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int DateCompleted { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int TimeCompleted { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int ErrorCode { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int AuthorizationNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public int? ManualEntry { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal CardId { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string VehicleIdentifier { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string SiteTaxLocation { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode1 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode2 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode3 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode4 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode5 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode6 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode7 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode8 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode9 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxCode10 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount1 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount2 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount3 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount4 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount5 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount6 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount7 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount8 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount9 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal? SiteTaxAmount10 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string TransactionLocationIndicator { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string NetworkIndicator { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal NetworkRate { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal PumpPrice { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal HaulRate { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public decimal CFNPrice { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string JobNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string POInvoiceNumber { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string CustomPrompt1 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string CustomPromptValue1 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string CustomPrompt2 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string CustomPromptValue2 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string CustomPrompt3 { get; set; }

            [FieldQuoted('"', QuoteMode.OptionalForBoth)]
            [FieldOptional]
            public string CustomPromptValue3 { get; set; }

            [FieldOptional]
            public string NameOnCard { get; set; }

            [FieldOptional]
            [FieldNullValue(0)]
            public int DeliveryId { get; set; }

            [FieldOptional]
            [FieldNullValue(null)]
            public Guid TransactionUID { get; set; }
        }
    }
}
