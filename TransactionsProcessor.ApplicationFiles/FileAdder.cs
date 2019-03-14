using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles.Core;

namespace TransactionsProcessor.ApplicationFiles
{
    public interface IFileAdder
    {
        Task<int> AddFile(string contentType, string fileName);
    }

    public class FileAdder : IFileAdder
    {
        private readonly IApplicationFilesDatabase _database;

        public FileAdder(IApplicationFilesDatabase database)
        {
            _database = database;
        }

        public async Task<int> AddFile(string contentType, string fileName)
        {
            return await _database.QuerySingle<int>("", new { @ContentType = contentType, @FileName = fileName });
        }
    }
}
