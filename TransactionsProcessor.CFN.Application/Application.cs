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
            var downloadFilesCommand = new DownloadFiles.Command
            {
                ContentType = "CFN"
            };

            var downloadedFilesResult = await _mediator.Send(downloadFilesCommand, cancellationToken);
        }

        private async Task PostProcess(CancellationToken cancellationToken)
        {

        }
    }
}
