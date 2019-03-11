using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Features;
using TransactionsProcessor.CFN.Application.Models;

namespace TransactionsProcessor.CFN.Application
{
    public class Application
    {
        private readonly IMediator _mediator;

        public Application(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ImportFiles(CancellationToken cancellationToken)
        {
            var filesInProcess = await PreProcess(cancellationToken);

            foreach(var fileInProcess in filesInProcess)
            {
                await Process(fileInProcess, cancellationToken);
            }

            await PostProcess(cancellationToken);
        }

        private async Task<List<FilesInProcess>> PreProcess(CancellationToken cancellationToken)
        {
            var filesToBeProcessedCommand = new GetFilesToBeProcessed.Command
            {
                ContentType = "CFN"
            };

            var filesToBeProcessedResult = await _mediator.Send(filesToBeProcessedCommand, cancellationToken);

            var filesToBeProcessed = new List<FilesInProcess>();

            foreach(var fileToBeProcessed in filesToBeProcessedResult.FilesToBeProcessed)
            {
                filesToBeProcessed.Add(new FilesInProcess { FileName = fileToBeProcessed.FileName });
            }

            return filesToBeProcessed;
        }

        private async Task Process(FilesInProcess filesInProcess, CancellationToken cancellationToken)
        {
            var filesToDownloadCommand = new DownloadFiles.Command
            {
                ContentType = "CFN",
                FileToDownload = filesInProcess.FileName 
            };
            var downloadedFilesResult = await _mediator.Send(filesToDownloadCommand, cancellationToken);

            var parseFileCommand = new ProcessFiles.Command
            {
                FullName = downloadedFilesResult.DownloadedFiles[0]
            };
            var parseFileResult = await _mediator.Send(parseFileCommand, cancellationToken);

            var prepareTransactionsCommand = new PrepareTransactions.Command
            {
                CfnRecordsDictonary = parseFileResult.CfnRecordsDictonary
            };
            var prepareTransactionsResult = await _mediator.Send(prepareTransactionsCommand, cancellationToken);


        }

        private async Task PostProcess(CancellationToken cancellationToken)
        {

        }
    }
}
