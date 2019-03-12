namespace TransactionsProcessor.FileManager.Domain
{
    public class FileConfiguration
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string FolderPath { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public int? SubCompanyId { get; set; }

        public string Provider { get; set; }
    }
}
