using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionsProcessor.CFN.Application.Features.SelectFailedFiles
{
    public partial class SelectFailedFiles
    {
        public class FilesToBeProcessed
        {
            public int FileId { get; set; }

            public string FileName { get; set; }
        }

    }
}
