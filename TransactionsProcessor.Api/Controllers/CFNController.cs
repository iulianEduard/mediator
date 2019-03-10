using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TransactionsProcessor.Api.Controllers
{
    [Route("api/cfn")]
    public class CFNController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CFN()
        {
            return Ok();
        }
    }
}