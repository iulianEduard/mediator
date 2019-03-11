using Renci.SshNet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.RemoteFileChecker
{
    public class SftpRemoteFileChecker : IRemoteFileChecker
    {
        public async Task<List<RemoteFileChecker>> GetRemoteFiles(RemoteFileCheckerCredentials credentials)
        {
            var remoteFiles = new List<RemoteFileChecker>();

            await Task.Run(() =>
             {
                 using (var client = new SftpClient(credentials.Host, credentials.UserName, credentials.UserPassword))
                 {
                     client.Connect();

                     var directoryListing = client.ListDirectory(credentials.Directory);

                     foreach (var fileInDirectory in directoryListing)
                     {
                         remoteFiles.Add(new RemoteFileChecker { FileName = fileInDirectory.FullName });
                     }

                     client.Disconnect();
                 }
             });

            return remoteFiles;
        }
    }
}
