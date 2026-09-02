using System;
using System.Collections.Generic;
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api.Core.Services
{
    public class TourneyService : ITourneyService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IBoutScheduler _boutScheduler;

        public TourneyService(ApplicationDbContext dbContext, IBoutScheduler boutScheduler)
        {
            _dbContext = dbContext;
            _boutScheduler = boutScheduler;
        }

        public IQueryable<Tourney> GetAllTourneys()
        {
            return _dbContext.Tourneys
                .Include(t => t.RuleSet)
                    .ThenInclude(r => r.Weapon)
                .AsQueryable();
        }

        public async Task<ResultModel<IEnumerable<Tourney>>> ListAllAsync()
        {
            var tourneys = await _dbContext.Tourneys
                .Include(t => t.RuleSet)
                    .ThenInclude(r => r.Weapon)
                .Include(t => t.Entries)
                .Include(t => t.Matches)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return new ResultModel<IEnumerable<Tourney>> { Data = tourneys };
        }

        public async Task<ResultModel<Tourney>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<Tourney>();

            var tourney = await _dbContext.Tourneys
                .Include(t => t.RuleSet)
                    .ThenInclude(r => r.Weapon)
                .FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (tourney is null)
            {
                resultModel.Errors.Add("Tourney does not exist");
                return resultModel;
            }

            resultModel.Data = tourney;
            return resultModel;
        }

        public async Task<ResultModel<Tourney>> GetFullAsync(Guid id)
        {
            var resultModel = new ResultModel<Tourney>();

            var tourney = await _dbContext.Tourneys
                .Include(t => t.RuleSet)
                    .ThenInclude(r => r.Weapon)
                .Include(t => t.Entries)
                    .ThenInclude(e => e.Player)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.FirstEntry)
                        .ThenInclude(e => e.Player)
                .Include(t => t.Matches)
                    .ThenInclude(m => m.SecondEntry)
                        .ThenInclude(e => e.Player)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.Id.Equals(id));

            if (tourney is null)
            {
                resultModel.Errors.Add("Tourney does not exist");
                return resultModel;
            }

            resultModel.Data = tourney;
            return resultModel;
        }

        public async Task<bool> DoesTourneyIdExistsAsync(Guid id)
        {
            return await _dbContext.Tourneys.AnyAsync(t => t.Id.Equals(id));
        }

        public async Task<ResultModel<Tourney>> AddAsync(Tourney entity)
        {
            var resultModel = new ResultModel<Tourney>();

            var ruleSetExists = await _dbContext.RuleSets.AnyAsync(r => r.Id == entity.RuleSetId);
            if (!ruleSetExists)
            {
                resultModel.Errors.Add("That rule set does not exist");
                return resultModel;
            }

            entity.Status = TourneyStatus.Setup;
            entity.CreatedAt = DateTime.UtcNow;

            _dbContext.Tourneys.Add(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;
            return resultModel;
        }

        public async Task<ResultModel<Tourney>> UpdateAsync(Tourney entity)
        {
            var resultModel = new ResultModel<Tourney>();

            if (await DoesTourneyIdExistsAsync(entity.Id) == false)
            {
                resultModel.Errors.Add($"There is no tourney with the ID {entity.Id}");
                return resultModel;
            }

            _dbContext.Tourneys.Update(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;
            return resultModel;
        }

        public async Task<ResultModel<Tourney>> DeleteAsync(Tourney entity)
        {
            var resultModel = new ResultModel<Tourney>();

            // Matches reference entries, so they have to go first. The foreign keys are
            // deliberately Restrict rather than Cascade to keep this order explicit.
            var matches = await _dbContext.Matches
                .Where(m => m.TourneyId == entity.Id)
                .ToListAsync();
            _dbContext.Matches.RemoveRange(matches);

            var entries = await _dbContext.TourneyEntries
                .Where(e => e.TourneyId == entity.Id)
                .ToListAsync();
            _dbContext.TourneyEntries.RemoveRange(entries);

            _dbContext.Tourneys.Remove(entity);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entity;
            return resultModel;
        }

        public async Task<ResultModel<TourneyEntry>> AddEntryAsync(Guid tourneyId, Guid playerId)
        {
            var resultModel = new ResultModel<TourneyEntry>();

            var tourney = await _dbContext.Tourneys
                .Include(t => t.Entries)
                .FirstOrDefaultAsync(t => t.Id == tourneyId);

            if (tourney is null)
            {
                resultModel.Errors.Add("Tourney does not exist");
                return resultModel;
            }

            if (tourney.Status != TourneyStatus.Setup)
            {
                resultModel.Errors.Add("Players can only be added while the tourney is still in setup");
                return resultModel;
            }

            if (await _dbContext.Players.AnyAsync(p => p.Id == playerId) == false)
            {
                resultModel.Errors.Add("Player does not exist");
                return resultModel;
            }

            if (tourney.Entries.Any(e => e.PlayerId == playerId))
            {
                resultModel.Errors.Add("That player is already entered in this tourney");
                return resultModel;
            }

            var entry = new TourneyEntry
            {
                Id = Guid.NewGuid(),
                TourneyId = tourneyId,
                PlayerId = playerId,
                Position = tourney.Entries.Count == 0 ? 1 : tourney.Entries.Max(e => e.Position) + 1
            };

            _dbContext.TourneyEntries.Add(entry);
            await _dbContext.SaveChangesAsync();

            resultModel.Data = entry;
            return resultModel;
        }

        public async Task<ResultModel<TourneyEntry>> RemoveEntryAsync(Guid tourneyId, Guid entryId)
        {
            var resultModel = new ResultModel<TourneyEntry>();

            var tourney = await _dbContext.Tourneys
                .Include(t => t.Entries)
                .FirstOrDefaultAsync(t => t.Id == tourneyId);

            if (tourney is null)
            {
                resultModel.Errors.Add("Tourney does not exist");
                return resultModel;
            }

            if (tourney.Status != TourneyStatus.Setup)
            {
                resultModel.Errors.Add("Players can only be removed while the tourney is still in setup");
                return resultModel;
            }

            var entry = tourney.Entries.FirstOrDefault(e => e.Id == entryId);

            if (entry is null)
            {
                resultModel.Errors.Add("That player is not entered in this tourney");
                return resultModel;
            }

            _dbContext.TourneyEntries.Remove(entry);

            // Close the gap so positions stay 1..n with nothing missing.
            var remaining = tourney.Entries
                .Where(e => e.Id != entryId)
                .OrderBy(e => e.Position)
                .ToList();

            for (var i = 0; i < remaining.Count; i++)
                remaining[i].Position = i + 1;

            await _dbContext.SaveChangesAsync();

            resultModel.Data = entry;
            return resultModel;
        }

        public async Task<ResultModel<Tourney>> GenerateMatchesAsync(Guid tourneyId)
        {
            var resultModel = new ResultModel<Tourney>();

            var tourney = await _dbContext.Tourneys
                .Include(t => t.Entries)
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == tourneyId);

            if (tourney is null)
            {
                resultModel.Errors.Add("Tourney does not exist");
                return resultModel;
            }

            if (tourney.Status != TourneyStatus.Setup)
            {
                resultModel.Errors.Add("This tourney has already been started");
                return resultModel;
            }

            if (tourney.Entries.Count < 2)
            {
                resultModel.Errors.Add("A tourney needs at least two players before it can start");
                return resultModel;
            }

            var byPosition = tourney.Entries.ToDictionary(e => e.Position, e => e.Id);
            var order = _boutScheduler.BuildOrder(tourney.Entries.Count);

            var matches = new List<Match>();
            var sequence = 1;

            foreach (var (first, second) in order)
            {
                matches.Add(new Match
                {
                    Id = Guid.NewGuid(),
                    TourneyId = tourney.Id,
                    FirstEntryId = byPosition[first],
                    SecondEntryId = byPosition[second],
                    FirstScore = 0,
                    SecondScore = 0,
                    Status = MatchStatus.Scheduled,
                    Order = sequence++
                });
            }

            _dbContext.Matches.AddRange(matches);
            tourney.Status = TourneyStatus.Running;

            await _dbContext.SaveChangesAsync();

            resultModel.Data = tourney;
            return resultModel;
        }

        public async Task<ResultModel<Tourney>> FinishAsync(Guid tourneyId)
        {
            var resultModel = new ResultModel<Tourney>();

            var tourney = await _dbContext.Tourneys
                .Include(t => t.Matches)
                .FirstOrDefaultAsync(t => t.Id == tourneyId);

            if (tourney is null)
            {
                resultModel.Errors.Add("Tourney does not exist");
                return resultModel;
            }

            if (tourney.Status != TourneyStatus.Running)
            {
                resultModel.Errors.Add("Only a running tourney can be finished");
                return resultModel;
            }

            var unfinished = tourney.Matches.Count(m => m.Status != MatchStatus.Finished);
            if (unfinished > 0)
            {
                resultModel.Errors.Add($"{unfinished} match(es) still need a final score");
                return resultModel;
            }

            tourney.Status = TourneyStatus.Finished;
            await _dbContext.SaveChangesAsync();

            resultModel.Data = tourney;
            return resultModel;
        }
    }
}
