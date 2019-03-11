using System;

namespace TransactionsProcessor.CFN.Application.Services.RemoteFileChecker
{
    public static class RemoteFileCheckerFactory
    {
        public static IRemoteFileChecker GetByName(string type)
        {
            switch (type)
            {
                case "FTP":
                    return new FtpRemoteFileChecker();
                case "SSL":
                    return new SslFtpRemoteFileChecker();
                case "SFTP":
                    return new SftpRemoteFileChecker();
                default:
                    throw new Exception("Wrong transfer protocol provided!");
            }
        }
    }
}
