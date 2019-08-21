using Microsoft.AspNetCore.Mvc;

namespace CzechNationalBank.Web.Controllers
{
    /// <summary/>
    [Route("api/")]
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary/>
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("~/api/help");
        }
    }
}