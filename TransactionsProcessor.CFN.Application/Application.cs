using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application.Features;

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
            var downloadFilesCommand = new DownloadFiles.Command
            {
                ContentType = "CFN"
            };

            var downloadedFilesResult = await _mediator.Send(downloadFilesCommand, cancellationToken);
        }
    }
}
