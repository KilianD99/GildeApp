using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mvc.GildeApp.mvc.Models;
using Mvc.GildeApp.mvc.Services;

namespace Mvc.GildeApp.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGildeApiClient _api;

        public HomeController(IGildeApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var tourneys = await _api.GetTourneysAsync(ct);

            var model = new TourneyListViewModel
            {
                Running = tourneys.Where(t => t.Status == "Running").ToList(),
                Setup = tourneys.Where(t => t.Status == "Setup").ToList(),
                Finished = tourneys.Where(t => t.Status == "Finished").ToList()
            };

            return View(model);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
