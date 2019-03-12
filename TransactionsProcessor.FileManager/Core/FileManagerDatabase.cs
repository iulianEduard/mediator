using TransactionsProcessor.Data;

namespace TransactionsProcessor.FileManager.Core
{
    public interface IFileManagerDatabase : IDatabase
    {

    }

    public class FileManagerDatabase : Database, IFileManagerDatabase
    {
        public FileManagerDatabase(FileManagerSettings settings) : base(new DatabaseConnection(settings.ConnectionString))
        {
        }
    }
}
