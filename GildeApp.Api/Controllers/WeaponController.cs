using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    public class WeaponController : ControllerBase
    {
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
