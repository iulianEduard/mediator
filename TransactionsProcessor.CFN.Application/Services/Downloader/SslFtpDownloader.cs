using FluentFTP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
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

            var downloadResponse = new DownloadResponse
            {
                Details = new List<DownloadResponseDetail>()
            };

            foreach (var fileToDownload in downloadRequest.FilesToDownload)
            {
                var fileName = Utils.GetFileNameFromFTP(fileToDownload);

                try
                {
                    var fileLocation = $@"{downloadRequest.DownloadLocation}{fileName}";

                    await client.DownloadFileAsync(fileLocation, fileToDownload);

                    downloadResponse.Details.Add(new DownloadResponseDetail { FileName = fileName, Error = "Success" });
                }
                catch (Exception ex)
                {
                    downloadResponse.Details.Add(new DownloadResponseDetail { FileName = fileName, Error = ex.InnerException?.Message ?? ex.Message });
                }
            }

            await client.DisconnectAsync();

            return downloadResponse;
        }

        private void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            // add logic to test if certificate is valid here
            e.Accept = true;
        }
    }
}
