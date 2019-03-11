using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.Downloader
{
    public interface IDownloader
    {
        Task<DownloadResponse> DownloadFilesAsync(DownloadRequest downloadRequest);
    }

    public class DownloadRequest
    {
        public DownloadSettings Options { get; set; }

        public string DownloadLocation { get; set; }

        public List<string> FilesToDownload { get; set; }

        public List<string> FilesToExclude { get; set; }

        public List<string> ExtensionsToDownload { get; set; }

        public List<string> ExtenionsToExclude { get; set; }

        public bool DeleteFilesAfterDownload { get; set; }
    }

    public class DownloadResponse
    {
        public List<DownloadResponseDetail> Details { get; set; }
    }

    public class DownloadResponseDetail
    {
        public string FileName { get; set; }

        public string Error { get; set; }

        public bool IsSuccess => Error == null;

        public static DownloadResponseDetail Success(string fileName)
        {
            return new DownloadResponseDetail { FileName = fileName };
        }

        public static DownloadResponseDetail Fail(string error)
        {
            return new DownloadResponseDetail { Error = error };
        }
    }

    public class DownloadSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Directory { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public string TransferProtocol { get; set; }
    }
}
