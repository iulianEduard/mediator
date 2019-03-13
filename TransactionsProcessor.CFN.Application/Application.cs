using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Features;
using TransactionsProcessor.CFN.Application.Features.Commit;
using TransactionsProcessor.CFN.Application.Features.Export;
using TransactionsProcessor.CFN.Application.Features.Parse;
using TransactionsProcessor.CFN.Application.Features.Transform;
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

            foreach (var fileInProcess in filesInProcess)
            {
                await Process(fileInProcess, cancellationToken);
            }

            await PostProcess(cancellationToken);
        }

        private async Task<List<FilesInProcess>> PreProcess(CancellationToken cancellationToken)
        {
            var filesToBeProcessedResult = await _mediator.Send(new SelectFiles.Command
            {
                ContentType = "CFN"
            }, cancellationToken);

            var filesToBeProcessed = new List<FilesInProcess>();

            foreach (var fileToBeProcessed in filesToBeProcessedResult.FilesToBeProcessed)
            {
                filesToBeProcessed.Add(new FilesInProcess { FileName = fileToBeProcessed.FileName });
            }

            return filesToBeProcessed;
        }

        private async Task Process(FilesInProcess filesInProcess, CancellationToken cancellationToken)
        {
            var downloadedFilesResult = await _mediator.Send(new DownloadFiles.Command
            {
                ContentType = "CFN",
                FileToDownload = filesInProcess.FileName
            }, cancellationToken);

            var parseResult = await _mediator.Send(new Parse.Command
            {
                FullName = downloadedFilesResult.DownloadedFiles[0]
            }, cancellationToken);

            var transformResult = await _mediator.Send(new Transform.Command
            {
                ParseTransactions = parseResult.Transactions
            }, cancellationToken);

            var commitResult = await _mediator.Send(new Commit.Command
            {
                ParseTransactions = transformResult.ParseTransactions
            }, cancellationToken);

            var exportResult = await _mediator.Send(new Export.Command
            {
                ContentType = "CFN",
                ParseModel = commitResult.ParseModel
            }, cancellationToken);

        }

        private async Task PostProcess(CancellationToken cancellationToken)
        {

        }
    }
}
