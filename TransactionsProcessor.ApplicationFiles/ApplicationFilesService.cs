using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles.Core;

namespace TransactionsProcessor.ApplicationFiles
{
    public class ApplicationFilesService : IApplicationFilesService
    {
        private readonly IApplicationFilesDatabase _database;

        public ApplicationFilesService(IApplicationFilesDatabase database)
        {
            _database = database;
        }

        public async Task<bool> ChangeStatus(int applicationFileId, string message, bool isError)
        {
            await _database.QuerySingle<int>("", new { applicationFileId, message, isError});

            return true;
        }

        public async Task<bool> CheckIfFileExists(string fileName)
        {
            return await _database.QuerySingle<bool>("", new { fileName });
        }

        public async Task<int> Insert(string fileName, string contentType)
        {
            return await _database.QuerySingle<int>("", new { fileName, contentType });
        }
    }
}
