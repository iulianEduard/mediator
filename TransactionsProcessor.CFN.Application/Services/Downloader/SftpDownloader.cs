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

                var filesToDownload = await ProcessDownloadFromFtpRequest(client, downloadRequest);

                foreach (var file in filesToDownload)
                {
                    try
                    {
                        var fileLocation = Path.Combine(downloadRequest.DownloadLocation, file.Name);

                        using (Stream fileStream = File.OpenWrite(fileLocation))
                        {
                            await client.DownloadAsync(file.FullName, fileStream);
                            downloadResponse.Details.Add(new DownloadResponseDetail { FileName = file.Name, Error = "Success" });
                        }
                    }
                    catch (Exception ex)
                    {
                        downloadResponse.Details.Add(new DownloadResponseDetail { FileName = file.Name, Error = ex.InnerException?.Message ?? ex.Message });
                    }
                }

                client.Disconnect();

                return downloadResponse;
            }
        }

        private async Task<IEnumerable<SftpFile>> ProcessDownloadFromFtpRequest(SftpClient client, DownloadRequest request)
        {
            if (request.FilesToDownload.Any())
            {
                var directoryListing = await client.ListDirectoryAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => request.FilesToDownload.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            if (request.FilesToExclude.Any())
            {
                var directoryListing = await client.ListDirectoryAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => !request.FilesToExclude.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            if (request.ExtensionsToDownload.Any())
            {
                var directoryListing = await client.ListDirectoryAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => request.ExtensionsToDownload.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            if (request.ExtenionsToExclude.Any())
            {
                var directoryListing = await client.ListDirectoryAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => request.ExtenionsToExclude.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            return await client.ListDirectoryAsync(request.Options.Directory);
        }
    }
}
