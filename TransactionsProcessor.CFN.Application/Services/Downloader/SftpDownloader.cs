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

                Utils.ValidateFolderLocation(downloadRequest.DownloadLocation);

                var downloadResponse = new DownloadResponse();

                foreach (var file in downloadRequest.FilesToDownload)
                {
                    var fileName = Utils.GetFileNameFromFTP(file);

                    try
                    {
                        var fileLocation = Path.Combine(downloadRequest.DownloadLocation, fileName);

                        using (Stream fileStream = File.OpenWrite(fileLocation))
                        {
                            await client.DownloadAsync(file, fileStream);
                            downloadResponse.Details.Add(new DownloadResponseDetail { FileName = fileName, Error = "Success" });
                        }
                    }
                    catch (Exception ex)
                    {
                        downloadResponse.Details.Add(new DownloadResponseDetail { FileName = fileName, Error = ex.InnerException?.Message ?? ex.Message });
                    }
                }

                client.Disconnect();

                return downloadResponse;
            }
        }
    }
}
