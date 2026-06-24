using GildeApp.Api.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GildeApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourneyController : ControllerBase
    {
        protected readonly ITourneyService _tourneyService;

        public TourneyController(ITourneyService tourneyService)
        {
            _tourneyService = tourneyService;
        }

        public IActionResult Index()
        {
            return Ok();
        }
    }
}
