using FluentFTP;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.RemoteFileChecker
{
    public class FtpRemoteFileChecker : IRemoteFileChecker
    {
        public async Task<List<RemoteFileChecker>> GetRemoteFiles(RemoteFileCheckerCredentials credentials)
        {
            var client = new FtpClient(credentials.Host)
            {
                Credentials = new System.Net.NetworkCredential(credentials.UserName, credentials.UserPassword)
            };

            await client.ConnectAsync();

            var directoryListing = await client.GetListingAsync(credentials.Directory);
            var remoteFiles = new List<RemoteFileChecker>();

            foreach(var fileInDirectory in directoryListing)
            {
                remoteFiles.Add(new RemoteFileChecker { FileName = fileInDirectory.FullName });
            }

            await client.DisconnectAsync();

            return remoteFiles;
        }
    }
}
