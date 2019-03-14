using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application;

namespace TransactionsProcessor.Api.Controllers
{
    [Route("api/cfn")]
    public class CFNController : Controller
    {
        private readonly AutomatedImport _application;

        public CFNController(AutomatedImport application)
        {
            _application = application;
        }

        [HttpPost]
        public async Task CFN(CancellationToken cancellationToken) => await _application.ImportFiles(cancellationToken);
    }
}