namespace TransactionsProcessor.CFN.Application.Models
{
    public class FileInProcess
    {
        public string FileName { get; set; }

        public int FileId { get; set; }

        public bool AreTransactionsCommited { get; set; }
    }
}
