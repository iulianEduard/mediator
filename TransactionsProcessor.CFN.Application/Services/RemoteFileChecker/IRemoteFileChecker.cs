using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.RemoteFileChecker
{
    public interface IRemoteFileChecker
    {
        Task<List<RemoteFileInfo>> GetRemoteFiles(RemoteFileRequest request);
    }

    public class RemoteFileRequest
    {
        public RemoteFileCredentials Credentials { get; set; }

        public RemoteFileOptions Options { get; set; }
    }

    public class RemoteFileCredentials
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Directory { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }
    }

    public class RemoteFileOptions
    {
        public List<string> FilesToDownload { get; set; }

        public List<string> FilesToExlude { get; set; }

        public List<string> ExtensionsToDownload { get; set; }

        public List<string> ExtensionsToExclude { get; set; }

        public bool DeleteAfterDownload { get; set; }
    }

    public class RemoteFileInfo
    {
        public string FileName { get; set; }

        public DateTime Created { get; set; }
        
        public DateTime Modified { get; set; }
    }
}
