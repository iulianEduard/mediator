using TransactionsProcessor.Data;

namespace TransactionsProcessor.CFN.Application.Core
{
    public class CfnDatabase : Database, ICfnDatabase
    {
        public CfnDatabase(CfnSettings settings) : base(new DatabaseConnection(settings.ConnectionString))
        {
        }
    }

    public interface ICfnDatabase : IDatabase
    {
    }
}
