using System.Collections.Generic;
using TransactionsProcessor.CFN.Application.Features;
using TransactionsProcessor.CFN.Application.Features.SelectFiles;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application.Core.Extensions
{
    public static class AutomatedImportExtensions
    {
        public static Response ToResponse(this SelectFiles.Result filesToBeProcessedResult)
        {
            var response = new Response
            {
                FileStatuses = new List<FileStatus>(),
                IsProcessFail = false
            };

            foreach (var fileToBeProcessed in filesToBeProcessedResult.FilesToBeProcessed)
            {
                response.FileStatuses.Add(new FileStatus
                {
                    FileName = fileToBeProcessed.FileName
                });
            }

            return response;
        }

        public static void ToUpdateResponse(this DownloadFiles.Result downloadFileResult, FileStatus fileStatus)
        {
            fileStatus.FileId = downloadFileResult.FileId;
            fileStatus.FileName = downloadFileResult.FileName;
            fileStatus.ProcessId = downloadFileResult.ProcessId;
        }
    }
}
