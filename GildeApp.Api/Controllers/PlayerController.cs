using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    public class PlayerController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
