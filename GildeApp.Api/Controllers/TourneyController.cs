using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    public class TourneyController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
