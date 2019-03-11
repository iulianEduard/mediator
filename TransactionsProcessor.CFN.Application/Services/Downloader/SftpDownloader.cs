using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TransactionsProcessor.Infrastructure.Helpers;

namespace TransactionsProcessor.CFN.Application.Services.Downloader
{
    public class SftpDownloader : IDownloader
    {
        public async Task<DownloadResponse> DownloadFilesAsync(DownloadRequest downloadRequest)
        {
            var credentials = downloadRequest.Options;

            using (SftpClient client = new SftpClient(credentials.Host, credentials.Port, credentials.UserName, credentials.UserPassword))
            {
                client.Connect();

                IEnumerable<SftpFile> files = null;

                if (!string.IsNullOrEmpty(credentials.Directory))
                {
                    files = await client.ListDirectoryAsync(credentials.Directory);
                }

                if (downloadRequest.FilesToDownload.Any())
                {
                    files = GetRequestedFiles(downloadRequest, files);
                }

                Utils.ValidateFolderLocation(downloadRequest.DownloadLocation);

                var downloadResponse = new DownloadResponse();

                foreach (var file in files)
                {
                    try
                    {
                        var fileLocation = Path.Combine(downloadRequest.DownloadLocation, file.Name);

                        //if (await CheckIfFileExistsInDatabase(fileLocation))
                        //{
                        //    continue;
                        //}

                        using (Stream fileStream = File.OpenWrite(fileLocation))
                        {
                            await client.DownloadAsync(file.FullName, fileStream);
                            downloadResponse.Details.Add(new DownloadResponseDetail { FileName = file.Name, Error = "Success" });
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        downloadResponse.Details.Add(new DownloadResponseDetail { FileName = file.Name, Error = errorMessage });
                    }
                }

                return downloadResponse;
            }
        }

        private IEnumerable<SftpFile> GetRequestedFiles(DownloadRequest request, IEnumerable<SftpFile> files)
        {
            IEnumerable<SftpFile> requestedFiles = null;

            requestedFiles = files.Where(f => request.FilesToDownload.Contains(f.FullName)).ToList();

            return requestedFiles;
        }
    }
}
