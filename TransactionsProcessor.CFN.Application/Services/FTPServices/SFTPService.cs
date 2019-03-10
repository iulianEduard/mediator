using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.FTPServices
{
    public class SFTPService : ISSHService
    {
        public Task<List<DownloadResponse>> DownloadFilesAsync(DownloadRequest downloadRequest)
        {
            throw new NotImplementedException();
        }

        public Task<List<UploadResponse>> UploadFilesAsync(UploadRequest uploadRequest)
        {
            throw new NotImplementedException();
        }
    }
}
