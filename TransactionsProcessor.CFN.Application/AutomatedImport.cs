﻿using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Features;
using TransactionsProcessor.CFN.Application.Features.Commit;
using TransactionsProcessor.CFN.Application.Features.Export;
using TransactionsProcessor.CFN.Application.Features.Finalize;
using TransactionsProcessor.CFN.Application.Features.Parse;
using TransactionsProcessor.CFN.Application.Features.SendToQC;
using TransactionsProcessor.CFN.Application.Features.Transform;
using TransactionsProcessor.CFN.Application.Features.Rollback;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application
{
    public class AutomatedImport
    {
        private readonly IMediator _mediator;

        public AutomatedImport(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Response> ImportFiles(CancellationToken cancellationToken)
        {
            var response = await PreProcess(cancellationToken);

            foreach (var fileStatus in response.FileStatuses)
            {
                try
                {
                    await Process(fileStatus, cancellationToken);
                }
                catch
                {
                    await Rollback(fileStatus, cancellationToken);
                }
            }

            return response;
        }

        private async Task<Response> PreProcess(CancellationToken cancellationToken)
        {
            var filesToBeProcessedResult = await _mediator.Send(new SelectFiles.Command
            {
                ContentType = "CFN"
            }, cancellationToken);

            var filesToBeProcessed = filesToBeProcessedResult.ToResponse();

            return filesToBeProcessed;
        }

        private async Task Process(FileStatus fileStatus, CancellationToken cancellationToken)
        {
            var downloadedFilesResult = await _mediator.Send(new DownloadFiles.Command
            {
                ContentType = "CFN",
                FileToDownload = fileStatus.FileName
            }, cancellationToken);
            downloadedFilesResult.ToUpdateResponse(fileStatus);

            var parseResult = await _mediator.Send(new Parse.Command
            {
                FullName = fileStatus.FileName
            }, cancellationToken);

            var transformResult = await _mediator.Send(new Transform.Command
            {
                ParseTransactions = parseResult.Transactions
            }, cancellationToken);

            var commitResult = await _mediator.Send(new Commit.Command
            {
                ParseTransactions = transformResult.ParseTransactions,
                ProcessId = fileStatus.ProcessId
            }, cancellationToken);

            var exportResult = await _mediator.Send(new Export.Command
            {
                ContentType = "CFN",
                ParseModel = commitResult.ParseModel
            }, cancellationToken);

            var rawResult = await _mediator.Send(new Finalize.Command
            {
            }, cancellationToken);

            var sendToQcResult = await _mediator.Send(new SendToQC.Command
            {
                ProcessId = fileStatus.ProcessId
            }, cancellationToken);

        }

        private async Task Rollback(FileStatus fileStatus, CancellationToken cancellationToken)
        {
            var rollbackResult = await _mediator.Send(new Rollback.Command
            {

            }, cancellationToken);
        }
    }

    public static class ApplicationExtensions
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
