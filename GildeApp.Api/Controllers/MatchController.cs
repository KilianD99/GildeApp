using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    public class MatchController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
