namespace TransactionsProcessor.CFN.Application.Features.SelectFiles
{
    public partial class SelectFiles
    {
        public class Credentials
        {
            public string Host { get; set; }

            public int Port { get; set; }

            public string TransferProtocol { get; set; }

            public string Directory { get; set; }

            public string UserName { get; set; }

            public string UserPassword { get; set; }
        }

        public class FileCheck
        {
            public string File { get; set; }

            public bool Imported { get; set; }
        }
    }
}
