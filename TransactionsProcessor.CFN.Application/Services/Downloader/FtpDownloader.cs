using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Services.Common;
using TransactionsProcessor.Infrastructure.Helpers;

namespace TransactionsProcessor.CFN.Application.Services.Downloader
{
    public class FtpDownloader : IDownloader
    {
        public async Task<DownloadResponse> DownloadFilesAsync(DownloadRequest downloadRequest)
        {
            var credentials = downloadRequest.Options;

            return await Task.Run(() =>
            {
                var host = $"ftp://{credentials.Host}{credentials.Directory}";
                var ftpRequest = SshUtils.CreateSSHRequest(host, false, WebRequestMethods.Ftp.ListDirectory, credentials);
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

        private string DownloadFile(DownloadRequest request, DownloadSettings settings, string fileName)
        {
            var host = $"ftp://{settings.Host}{settings.Directory}/{fileName}";
            var ftpRequest = SshUtils.CreateSSHRequest(host, false, WebRequestMethods.Ftp.DownloadFile, settings);
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
    }
}
