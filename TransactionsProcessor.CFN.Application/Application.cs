using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> ImportFiles(CancellationToken cancellationToken)
        {
            var downloadFilesCommand = new DownloadFiles.Command
            {
                ContentType = "CFN"
            };

            var downloadedFilesResult = await _mediator.Send(downloadFilesCommand, cancellationToken);

            var response = new OkResult();
            return response;
        }
    }
}
