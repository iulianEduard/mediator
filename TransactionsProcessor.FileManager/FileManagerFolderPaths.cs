using System.Threading.Tasks;
using TransactionsProcessor.FileManager.Core;
using TransactionsProcessor.FileManager.Domain;

namespace TransactionsProcessor.FileManager
{
    public interface IFileManagerFolderPaths
    {
        Task<FolderPath> GetFolderPath(string contentType);
    }

    public class FileManagerFolderPaths : IFileManagerFolderPaths
    {
        private readonly IFileManagerDatabase _database;

        public FileManagerFolderPaths(IFileManagerDatabase database)
        {
            _database = database;
        }

        public async Task<FolderPath> GetFolderPath(string contentType)
        {
            var response = await _database.QuerySingle<FolderPath>("fss.usp_GetFolderPathByContentType", new { contentType });

            return response;
        }
    }
}
