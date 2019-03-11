using System.Threading.Tasks;

namespace TransactionsProcessor.ApplicationFiles
{
    public interface IApplicationFilesService
    {
        Task<int> Insert(string fileName, string contentType);

        Task<bool> ChangeStatus(int applicationFileId, string message, bool isError);

        Task<bool> CheckIfFileExists(string fileName);
    }
}
