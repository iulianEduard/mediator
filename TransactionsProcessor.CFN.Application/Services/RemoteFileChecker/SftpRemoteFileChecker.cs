using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.RemoteFileChecker
{
    public class SftpRemoteFileChecker : IRemoteFileChecker
    {
        public async Task<List<RemoteFileInfo>> GetRemoteFiles(RemoteFileRequest request)
        {
            var credentials = request.Credentials;
            var remoteFiles = new List<RemoteFileInfo>();

            await Task.Run(() =>
             {
                 using (var client = new SftpClient(credentials.Host, credentials.UserName, credentials.UserPassword))
                 {
                     client.Connect();

                     var directoryListing = GetDirectoryListAsync(client, request);

                     foreach (var fileInDirectory in directoryListing)
                     {
                         remoteFiles.Add(new RemoteFileInfo { FileName = fileInDirectory.FullName });
                     }

                     client.Disconnect();
                 }
             });

            return remoteFiles;
        }

        private IEnumerable<SftpFile> GetDirectoryListAsync(SftpClient client, RemoteFileRequest request)
        {
            var options = request.Options;
            var directory = request.Credentials.Directory;

            if (options.FilesToDownload.Any())
            {
                var directoryListing = client.ListDirectory(request.Credentials.Directory);
                return directoryListing.Where(dl => options.FilesToDownload.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            if (options.FilesToExlude.Any())
            {
                var directoryListing = client.ListDirectory(request.Credentials.Directory);
                return directoryListing.Where(dl => !options.FilesToExlude.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            if (options.ExtensionsToDownload.Any())
            {
                var directoryListing = client.ListDirectory(request.Credentials.Directory);
                return directoryListing.Where(dl => options.ExtensionsToDownload.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            if (options.ExtensionsToExclude.Any())
            {
                var directoryListing = client.ListDirectory(request.Credentials.Directory);
                return directoryListing.Where(dl => !options.ExtensionsToExclude.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            return client.ListDirectory(request.Credentials.Directory);
        }
    }
}
