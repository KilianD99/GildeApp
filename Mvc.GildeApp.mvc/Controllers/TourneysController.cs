using Microsoft.AspNetCore.Mvc;
using Mvc.GildeApp.mvc.Models;
using Mvc.GildeApp.mvc.Services;

namespace Mvc.GildeApp.mvc.Controllers
{
    public class TourneysController : Controller
    {
        private readonly IGildeApiClient _api;

        public TourneysController(IGildeApiClient api)
        {
            _api = api;
        }

        // ---- create --------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = new CreateTourneyViewModel
            {
                AvailableRuleSets = await _api.GetRuleSetsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTourneyViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRuleSets = await _api.GetRuleSetsAsync(ct);
                return View(model);
            }

            var result = await _api.CreateTourneyAsync(model.Name, model.RuleSetId!.Value, ct);

            if (!result.IsSuccess || result.Data is null)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error);

                model.AvailableRuleSets = await _api.GetRuleSetsAsync(ct);
                return View(model);
            }

            return RedirectToAction(nameof(Entries), new { id = result.Data.TourneyId });
        }

        // ---- entries -------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Entries(Guid id, CancellationToken ct)
        {
            var tourney = await _api.GetTourneyAsync(id, ct);

            if (tourney is null)
                return NotFound();

            // A tourney that has already started has nothing left to edit here.
            if (tourney.Status != "Setup")
                return RedirectToAction(nameof(Board), new { id });

            return View(await BuildEntriesModel(tourney, ct));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExistingPlayer(Guid id, Guid playerId, CancellationToken ct)
        {
            var result = await _api.AddEntryAsync(id, playerId, ct);

            if (!result.IsSuccess)
                TempData["Error"] = string.Join(" ", result.Errors);

            return RedirectToAction(nameof(Entries), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewPlayer(Guid id, string newFirstName, string newLastName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(newFirstName) || string.IsNullOrWhiteSpace(newLastName))
            {
                TempData["Error"] = "A new player needs both a first and a last name.";
                return RedirectToAction(nameof(Entries), new { id });
            }

            var created = await _api.CreatePlayerAsync(newFirstName.Trim(), newLastName.Trim(), ct);

            if (!created.IsSuccess || created.Data is null)
            {
                TempData["Error"] = string.Join(" ", created.Errors);
                return RedirectToAction(nameof(Entries), new { id });
            }

            // Creating the person and entering them are two calls: the player now exists
            // on file and can be picked again for the next tourney.
            var entered = await _api.AddEntryAsync(id, created.Data.PlayerId, ct);

            if (!entered.IsSuccess)
                TempData["Error"] = string.Join(" ", entered.Errors);

            return RedirectToAction(nameof(Entries), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveEntry(Guid id, Guid entryId, CancellationToken ct)
        {
            var result = await _api.RemoveEntryAsync(id, entryId, ct);

            if (!result.IsSuccess)
                TempData["Error"] = string.Join(" ", result.Errors);

            return RedirectToAction(nameof(Entries), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(Guid id, CancellationToken ct)
        {
            var result = await _api.GenerateAsync(id, ct);

            if (!result.IsSuccess)
            {
                TempData["Error"] = string.Join(" ", result.Errors);
                return RedirectToAction(nameof(Entries), new { id });
            }

            return RedirectToAction(nameof(Board), new { id });
        }

        // ---- board ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Board(Guid id, CancellationToken ct)
        {
            var board = await _api.GetBoardAsync(id, ct);

            if (board is null)
                return NotFound();

            return View(board);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finish(Guid id, CancellationToken ct)
        {
            var result = await _api.FinishAsync(id, ct);

            if (!result.IsSuccess)
                TempData["Error"] = string.Join(" ", result.Errors);

            return RedirectToAction(nameof(Board), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var result = await _api.DeleteTourneyAsync(id, ct);

            if (!result.IsSuccess)
                TempData["Error"] = string.Join(" ", result.Errors);

            return RedirectToAction("Index", "Home");
        }

        private async Task<EntriesViewModel> BuildEntriesModel(TourneyDetailModel tourney, CancellationToken ct)
        {
            var allPlayers = await _api.GetPlayersAsync(null, ct);
            var enteredIds = tourney.Entries.Select(e => e.PlayerId).ToHashSet();

            return new EntriesViewModel
            {
                Tourney = tourney,
                AvailablePlayers = allPlayers.Where(p => !enteredIds.Contains(p.PlayerId)).ToList()
            };
        }
    }
}
