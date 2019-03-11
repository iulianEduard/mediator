using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Services.Common;
using TransactionsProcessor.Infrastructure.Helpers;

namespace TransactionsProcessor.CFN.Application.Services.Downloader
{
    public class SslFtpDownloader : IDownloader
    {
        public async Task<DownloadResponse> DownloadFilesAsync(DownloadRequest downloadRequest)
        {
            var credentials = downloadRequest.Options;

            var client = new FtpClient(credentials.Host)
            {
                Credentials = new NetworkCredential(credentials.UserName, credentials.UserPassword),
                EncryptionMode = FtpEncryptionMode.Explicit,
                SslProtocols = SslProtocols.Tls
            };
            client.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);

            await client.ConnectAsync();

            var filesToDownload = await ProcessDownloadFromFtpRequest(client, downloadRequest);
            var downloadResponse = new DownloadResponse
            {
                Details = new List<DownloadResponseDetail>()
            };

            foreach (var fileToDownload in filesToDownload)
            {
                try
                {
                    var fileName = fileToDownload.Name;
                    var fileLocation = $@"{downloadRequest.DownloadLocation}{fileName}";

                    await client.DownloadFileAsync(fileLocation, fileToDownload.FullName);

                    downloadResponse.Details.Add(new DownloadResponseDetail { FileName = fileLocation, Error = "Success" });
                }
                catch (Exception ex)
                {
                    downloadResponse.Details.Add(new DownloadResponseDetail { FileName = fileToDownload.FullName, Error = ex.InnerException?.Message ?? ex.Message });
                }
            }

            await client.DisconnectAsync();

            return downloadResponse;
        }

        private async Task<IEnumerable<FtpListItem>> ProcessDownloadFromFtpRequest(FtpClient client, DownloadRequest request)
        {
            if (request.FilesToDownload.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => request.FilesToDownload.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            if (request.FilesToExclude.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => !request.FilesToExclude.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            if (request.ExtensionsToDownload.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => request.ExtensionsToDownload.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            if (request.ExtenionsToExclude.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Options.Directory);
                var filesToDownload = directoryListing.Where(d => request.ExtenionsToExclude.Any(r => d.FullName.ToLower().Contains(r)));

                return filesToDownload;
            }

            return await client.GetListingAsync(request.Options.Directory);
        }

        private void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            // add logic to test if certificate is valid here
            e.Accept = true;
        }
    }
}
