using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TransactionsProcessor.CFN.Application;

namespace TransactionsProcessor.Api.Controllers
{
    [Route("api/cfn")]
    public class CFNController : Controller
    {
        private readonly Application _application;

        public CFNController(Application application)
        {
            _application = application;
        }

        [HttpPost]
        public async Task<IActionResult> CFN(CancellationToken cancellationToken) => await _application.ImportFiles(cancellationToken);
    }
}