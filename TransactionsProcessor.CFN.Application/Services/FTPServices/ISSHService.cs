using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.FTPServices
{
    public interface ISSHService
    {
        Task<List<UploadResponse>> UploadFilesAsync(UploadRequest uploadRequest);

        Task<List<DownloadResponse>> DownloadFilesAsync(DownloadRequest downloadRequest);
    }

    public class UploadRequest
    {
        public FTPOptions Options { get; set; }

        public List<string> FilesToUploadList { get; set; }

        public bool DeleteFilesAfterUpload { get; set; }
    }

    public class UploadResponse
    {
        public string FileName { get; set; }

        public string ErrorDuringUpload { get; set; }
    }

    public class DownloadRequest
    {
        public FTPOptions Options { get; set; }

        public string DownloadLocation { get; set; }

        public List<string> FilesToDownloadList { get; set; }

        public List<string> FilesToExcludeList { get; set; }

        public List<string> ExtensionsToDownloadList { get; set; }

        public List<string> ExtenionsToExcludeList { get; set; }

        public bool DeleteFilesAfterDownload { get; set; }
    }

    public class DownloadResponse
    {
        public string FileName { get; set; }

        public string ErrorDuringDownload { get; set; }
    }

    public class FTPOptions
    {
        public string IP { get; set; }

        public string Location { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public string TransferProtocol { get; set; }
    }
}
