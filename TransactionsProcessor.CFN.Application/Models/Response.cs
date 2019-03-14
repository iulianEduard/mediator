using System;
using System.Collections.Generic;

namespace TransactionsProcessor.CFN.Application.Models
{
    public class Response
    {
        public List<FileStatus> FileStatuses { get; set; }

        public bool IsProcessFail { get; set; }
    }

    public class FileStatus
    {
        public string FileName { get; set; }

        public int FileId { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }

        public Guid ProcessId { get; set; }
    }

}
