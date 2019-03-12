using FluentFTP;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.RemoteFileChecker
{
    public class FtpRemoteFileChecker : IRemoteFileChecker
    {
        public async Task<List<RemoteFileInfo>> GetRemoteFiles(RemoteFileRequest request)
        {
            var credentials = request.Credentials;
            var options = request.Options;

            var client = new FtpClient(credentials.Host)
            {
                Credentials = new System.Net.NetworkCredential(credentials.UserName, credentials.UserPassword)
            };

            await client.ConnectAsync();

            var directoryListing = await GetDirectoryListAsync(client, request);
            var remoteFiles = new List<RemoteFileInfo>();

            // TODO: 
            foreach(var fileInDirectory in directoryListing)
            {
                remoteFiles.Add(new RemoteFileInfo { FileName = fileInDirectory.FullName });
            }

            await client.DisconnectAsync();

            return remoteFiles;
        }

        private async Task<IEnumerable<FtpListItem>> GetDirectoryListAsync(FtpClient client, RemoteFileRequest request)
        {
            var options = request.Options;
            var directory = request.Credentials.Directory;

            if(options.FilesToDownload.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Credentials.Directory);
                return directoryListing.Where(dl => options.FilesToDownload.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            if(options.FilesToExlude.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Credentials.Directory);
                return directoryListing.Where(dl => !options.FilesToExlude.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            if(options.ExtensionsToDownload.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Credentials.Directory);
                return directoryListing.Where(dl => options.ExtensionsToDownload.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            if(options.ExtensionsToExclude.Any())
            {
                var directoryListing = await client.GetListingAsync(request.Credentials.Directory);
                return directoryListing.Where(dl => !options.ExtensionsToExclude.Any(o => dl.Name.ToLower().Contains(dl.Name)));
            }

            return await client.GetListingAsync(request.Credentials.Directory);
        }
    }
}
