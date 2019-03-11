using TransactionsProcessor.Data;

namespace TransactionsProcessor.ApplicationFiles.Core
{
    public interface IApplicationFilesDatabase : IDatabase
    {
    }


    public class ApplicationFilesDatabase : Database, IApplicationFilesDatabase
    {
        public ApplicationFilesDatabase(ApplicationFilesSettings settings) : base(new DatabaseConnection(settings.ConnectionString))
        {
        }
    }
}
