namespace TransactionsProcessor.CFN.Application.Services.Downloader
{
    public static class DownloaderFactory
    {
        public static IDownloader Get(DownloaderType type)
        {
            switch(type)
            {
                case DownloaderType.SSL:
                    return new SslFtpDownloader();
                case DownloaderType.SFTP:
                    return new SftpDownloader();
                default:
                    return new FtpDownloader();
            }
        }

        public static IDownloader GetByName(string typeName)
        {
            switch (typeName.Trim().ToLower())
            {
                case "ssl":
                    return new SslFtpDownloader();
                case "sftp":
                    return new SftpDownloader();
                default:
                    return new FtpDownloader();
            }
        }
    }

    public enum DownloaderType
    {
        FTP,
        SSL,
        SFTP
    }
}
