using System.Threading.Tasks;
using TransactionsProcessor.FileManager.Core;
using TransactionsProcessor.FileManager.Domain;

namespace TransactionsProcessor.FileManager
{
    public interface IFileManagerFileConfigurations
    {
        Task<FileConfiguration> GetFileConfiguration(string contentType);
    }

    public class FileManagerFileConfigurations : IFileManagerFileConfigurations
    {
        private readonly IFileManagerDatabase _database;

        public FileManagerFileConfigurations(IFileManagerDatabase database)
        {
            _database = database;
        }

        public async Task<FileConfiguration> GetFileConfiguration(string contentType)
        {
            var response = await _database.QuerySingle<FileConfiguration>("fss.usp_GetFileSettingsByContentType", new { contentType });

            return response;
        }
    }
}
