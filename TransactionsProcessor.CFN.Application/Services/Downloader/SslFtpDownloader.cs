using System;
using System.IO;
using System.Net;
using System.Net.Security;
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

            return await Task.Run(() =>
            {
                var host = $"ftp://{credentials.Host}{credentials.Directory}";
                var ftpRequest = SshUtils.CreateSSHRequest(host, true, WebRequestMethods.Ftp.ListDirectory, credentials);

                RemoteCertificateValidationCallback orgCallback = null;
                orgCallback = ServicePointManager.ServerCertificateValidationCallback;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
                ServicePointManager.Expect100Continue = true;

                var response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader srFiles = new StreamReader(response.GetResponseStream());

                string fileLines = srFiles.ReadLine();
                var downloadResponse = new DownloadResponse();

                while (fileLines != null)
                {
                    var fileName = Utils.GetFileNameFromFTP(fileLines);

                    //if (await CheckIfFileExistsInDatabase(Path.Combine(downloadRequest.DownloadLocation, fileName)))
                    //{
                    //    fileLines = srFiles.ReadLine();
                    //    continue;
                    //}

                    var fileLocation = DownloadFile(downloadRequest, credentials, fileName);
                    var fileResponse = new DownloadResponseDetail
                    {
                        FileName = fileLocation,
                        Error = "Success"
                    };

                    downloadResponse.Details.Add(fileResponse);

                    fileLines = srFiles.ReadLine();
                }

                return downloadResponse;
            });
        }

        private string DownloadFile(DownloadRequest request, DownloadSettings credentials, string fileName)
        {
            var host = $"ftp://{credentials.Host}{credentials.Directory}/{fileName}";
            var ftpRequest = SshUtils.CreateSSHRequest(host, true, WebRequestMethods.Ftp.DownloadFile, credentials);

            RemoteCertificateValidationCallback orgCallback = null;
            orgCallback = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnValidateCertificate);
            ServicePointManager.Expect100Continue = true;

            var response = (FtpWebResponse)ftpRequest.GetResponse();

            Stream stream = null;
            StreamReader reader = null;
            StreamWriter writer = null;

            Utils.ValidateFolderLocation(request.DownloadLocation);

            var finalLocation = $@"{request.DownloadLocation}{fileName}";

            stream = response.GetResponseStream();
            using (reader = new StreamReader(stream, Encoding.UTF8))
            {
                writer = new StreamWriter(finalLocation, false);

                if (!reader.EndOfStream)
                {
                    writer.Write(reader.ReadToEnd());
                }

                writer.Close();
                stream.Close();
            }

            return finalLocation;
        }

        private bool OnValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
