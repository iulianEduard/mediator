using TransactionsProcessor.Data;

namespace TransactionsProcessor.CFN.Application.Core
{
    public interface IAfDatabase : IDatabase
    {
    }

    public class AfDatabase : Database, IAfDatabase
    {
        public AfDatabase(AfSettings settings) : base(new DatabaseConnection(settings.ConnectionString))
        {
        }
    }
}
