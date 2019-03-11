using System.Collections.Generic;
using System.Threading.Tasks;

namespace TransactionsProcessor.CFN.Application.Services.RemoteFileChecker
{
    public interface IRemoteFileChecker
    {
        Task<List<RemoteFileChecker>> GetRemoteFiles(RemoteFileCheckerCredentials credentials);
    }

    public class RemoteFileCheckerCredentials
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Directory { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }
    }

    public class RemoteFileChecker
    {
        public string FileName { get; set; }
    }
}
