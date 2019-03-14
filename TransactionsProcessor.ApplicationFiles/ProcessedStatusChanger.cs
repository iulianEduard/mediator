using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles.Core;

namespace TransactionsProcessor.ApplicationFiles
{
    public interface IChangeProcessedStatusChanger
    {
        Task ChangeProcessedStatus(int id, string message, bool success);

        Task ChangeProcessedStatusToSuccess(int id);

        Task ChangeProcessedStatusToFail(int id, string message);
    }

    public class ProcessedStatusChanger : IChangeProcessedStatusChanger
    {
        private readonly IApplicationFilesDatabase _database;

        public ProcessedStatusChanger(IApplicationFilesDatabase database)
        {
            _database = database;
        }

        public async Task ChangeProcessedStatus(int id, string message, bool success)
        {
            await _database.QuerySingle<int>("", new { id, message, success });
        }

        public async Task ChangeProcessedStatusToSuccess(int id)
        {
            await ChangeProcessedStatus(id, "Success", true);
        }

        public async Task ChangeProcessedStatusToFail(int id, string message)
        {
            await ChangeProcessedStatus(id, message, false);
        }
    }
}
