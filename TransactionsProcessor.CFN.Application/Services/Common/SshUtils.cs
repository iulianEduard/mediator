using System.Net;
using TransactionsProcessor.CFN.Application.Services.Downloader;

namespace TransactionsProcessor.CFN.Application.Services.Common
{
    public static class SshUtils
    {
        public static FtpWebRequest CreateSSHRequest(string host, bool enableSSL, string ftpMethod, DownloadSettings settings)
        {
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host);

            ftpRequest.Method = ftpMethod;
            ftpRequest.UsePassive = true;
            ftpRequest.UseBinary = true;
            ftpRequest.KeepAlive = false;
            ftpRequest.Credentials = new NetworkCredential(settings.UserName, settings.UserPassword);
            ftpRequest.EnableSsl = enableSSL;

            return ftpRequest;

        }
    }
}
