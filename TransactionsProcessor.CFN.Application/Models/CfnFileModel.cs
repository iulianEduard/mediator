namespace TransactionsProcessor.CFN.Application.Models
{
    public class CfnFileModel
    {
        public int SiteIdNumber { get; set; }

        public int SequenceNumber { get; set; }

        public int StatusCode { get; set; }

        public decimal TotalAmount { get; set; }

        public int TransactionType { get; set; }

        public int ProductCode { get; set; }

        public string ProductType { get; set; }

        public string ProductDescription { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public int Odometer { get; set; }

        public int PumpNumber { get; set; }

        public long TransactionNumber { get; set; }

        public int DateCompleted { get; set; }

        public int TimeCompleted { get; set; }

        public int ErrorCode { get; set; }

        public int AuthorizationNumber { get; set; }

        public int? ManualEntry { get; set; }

        public decimal CardId { get; set; }

        public string VehicleIdentifier { get; set; }

        public string SiteTaxLocation { get; set; }

        public decimal? SiteTaxCode1 { get; set; }

        public decimal? SiteTaxCode2 { get; set; }

        public decimal? SiteTaxCode3 { get; set; }

        public decimal? SiteTaxCode4 { get; set; }

        public decimal? SiteTaxCode5 { get; set; }

        public decimal? SiteTaxCode6 { get; set; }

        public decimal? SiteTaxCode7 { get; set; }

        public decimal? SiteTaxCode8 { get; set; }

        public decimal? SiteTaxCode9 { get; set; }

        public decimal? SiteTaxCode10 { get; set; }

        public decimal? SiteTaxAmount1 { get; set; }

        public decimal? SiteTaxAmount2 { get; set; }

        public decimal? SiteTaxAmount3 { get; set; }

        public decimal? SiteTaxAmount4 { get; set; }

        public decimal? SiteTaxAmount5 { get; set; }

        public decimal? SiteTaxAmount6 { get; set; }

        public decimal? SiteTaxAmount7 { get; set; }

        public decimal? SiteTaxAmount8 { get; set; }

        public decimal? SiteTaxAmount9 { get; set; }

        public decimal? SiteTaxAmount10 { get; set; }

        public string TransactionLocationIndicator { get; set; }

        public string NetworkIndicator { get; set; }

        public decimal NetworkRate { get; set; }

        public decimal PumpPrice { get; set; }

        public decimal HaulRate { get; set; }

        public decimal CFNPrice { get; set; }

        public string JobNumber { get; set; }

        public string POInvoiceNumber { get; set; }

        public string CustomPrompt1 { get; set; }

        public string CustomPromptValue1 { get; set; }

        public string CustomPrompt2 { get; set; }

        public string CustomPromptValue2 { get; set; }

        public string CustomPrompt3 { get; set; }

        public string CustomPromptValue3 { get; set; }

        public string NameOnCard { get; set; }

        public int DeliveryId { get; set; }
    }
}
