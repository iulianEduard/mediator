using System.Collections.Generic;
using System.Threading.Tasks;
using TransactionsProcessor.ApplicationFiles.Core;
using TransactionsProcessor.ApplicationFiles.Dto;

namespace TransactionsProcessor.ApplicationFiles
{
    public interface IImportedFileChecker
    {
        Task<IEnumerable<FileCheck>> Check(List<string> files); 
    }

    public class ImportedFileChecker : IImportedFileChecker
    {
        private readonly IApplicationFilesDatabase _database;

        public ImportedFileChecker(IApplicationFilesDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<FileCheck>> Check(List<string> files)
        {
            return await _database.Query<FileCheck>("", new { files });
        }
    }
}
