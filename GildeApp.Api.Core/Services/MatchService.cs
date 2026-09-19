using System;
using System.Collections.Generic;
using GildeApp.Api.Core.Data;
using GildeApp.Api.Core.Entities;
using GildeApp.Api.Core.Services.Interfaces;
using GildeApp.Api.Core.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace GildeApp.Api.Core.Services
{
    public class MatchService : IMatchService
    {
        public static readonly TimeSpan ClaimDuration = TimeSpan.FromMinutes(2);

        private readonly ApplicationDbContext _dbContext;

        public MatchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<Match> WithPlayers()
        {
            return _dbContext.Matches
                .Include(m => m.Tourney)
                    .ThenInclude(t => t.RuleSet)
                .Include(m => m.FirstEntry)
                    .ThenInclude(e => e.Player)
                .Include(m => m.SecondEntry)
                    .ThenInclude(e => e.Player);
        }

        public IQueryable<Match> GetAllMatches()
        {
            return WithPlayers().AsQueryable();
        }

        public async Task<ResultModel<IEnumerable<Match>>> ListAllAsync()
        {
            var matches = await WithPlayers()
                .OrderBy(m => m.TourneyId)
                .ThenBy(m => m.Order)
                .ToListAsync();

            return new ResultModel<IEnumerable<Match>> { Data = matches };
        }

        public async Task<ResultModel<IEnumerable<Match>>> ListForTourneyAsync(Guid tourneyId)
        {
            var resultModel = new ResultModel<IEnumerable<Match>>();

            if (await _dbContext.Tourneys.AnyAsync(t => t.Id == tourneyId) == false)
            {
                resultModel.Errors.Add("Tourney does not exist");
                return resultModel;
            }

            resultModel.Data = await WithPlayers()
                .Where(m => m.TourneyId == tourneyId)
                .OrderBy(m => m.Order)
                .ToListAsync();

            return resultModel;
        }

        public async Task<ResultModel<Match>> GetByIdAsync(Guid id)
        {
            var resultModel = new ResultModel<Match>();

            var match = await WithPlayers().FirstOrDefaultAsync(m => m.Id.Equals(id));

            if (match is null)
            {
                resultModel.Errors.Add("Match does not exist");
                return resultModel;
            }

            resultModel.Data = match;
            return resultModel;
        }

        public async Task<bool> DoesMatchIdExistsAsync(Guid id)
        {
            return await _dbContext.Matches.AnyAsync(m => m.Id.Equals(id));
        }
        public async Task<ResultModel<Match>> ClaimAsync(Guid matchId, string judgeName)
        {
            var resultModel = new ResultModel<Match>();

            var match = await WithPlayers().FirstOrDefaultAsync(m => m.Id == matchId);

            if (match is null)
            {
                resultModel.Errors.Add("Match does not exist");
                return resultModel;
            }

            if (match.Status == MatchStatus.Finished)
            {
                resultModel.Errors.Add("This match is already finished");
                return resultModel;
            }

            if (match.Tourney.Status != TourneyStatus.Running)
            {
                resultModel.Errors.Add("This tourney is not running");
                return resultModel;
            }

            if (!await TryTakeClaimAsync(matchId, judgeName))
            {
                resultModel.Errors.Add($"{match.ClaimedBy} is scoring this match right now");
                return resultModel;
            }

            await _dbContext.Entry(match).ReloadAsync();

            resultModel.Data = match;
            return resultModel;
        }

        private async Task<bool> TryTakeClaimAsync(Guid matchId, string judgeName)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(ClaimDuration);

            var rowsChanged = await _dbContext.Matches
                .Where(m => m.Id == matchId
                            && (m.ClaimedBy == null
                                || m.ClaimedBy == judgeName
                                || m.ClaimExpiresAt == null
                                || m.ClaimExpiresAt < now))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ClaimedBy, judgeName)
                    .SetProperty(m => m.ClaimExpiresAt, expires));

            return rowsChanged > 0;
        }

        public async Task<ResultModel<Match>> ReleaseAsync(Guid matchId, string judgeName)
        {
            var resultModel = new ResultModel<Match>();

            var match = await WithPlayers().FirstOrDefaultAsync(m => m.Id == matchId);

            if (match is null)
            {
                resultModel.Errors.Add("Match does not exist");
                return resultModel;
            }

            var heldByOther = match.IsClaimedAt(DateTime.UtcNow) && match.ClaimedBy != judgeName;

            if (!heldByOther && match.ClaimedBy is not null)
            {
                match.ClaimedBy = null;
                match.ClaimExpiresAt = null;
                await _dbContext.SaveChangesAsync();
            }

            resultModel.Data = match;
            return resultModel;
        }

        public async Task<ResultModel<Match>> SubmitScoreAsync(
            Guid matchId, int firstScore, int secondScore, bool finish, string judgeName)
        {
            var resultModel = new ResultModel<Match>();

            var match = await WithPlayers().FirstOrDefaultAsync(m => m.Id == matchId);

            if (match is null)
            {
                resultModel.Errors.Add("Match does not exist");
                return resultModel;
            }

            if (match.Status == MatchStatus.Finished)
            {
                resultModel.Errors.Add(
                    "This match has already been submitted. Reopen it before changing the score.");
                return resultModel;
            }

            if (match.Tourney.Status != TourneyStatus.Running)
            {
                resultModel.Errors.Add("Scores can only be entered while the tourney is running");
                return resultModel;
            }

            if (firstScore < 0 || secondScore < 0)
            {
                resultModel.Errors.Add("A score cannot be negative");
                return resultModel;
            }

            var maxScore = match.Tourney.RuleSet?.MaxScore ?? 0;
            if (maxScore > 0 && (firstScore > maxScore || secondScore > maxScore))
            {
                resultModel.Errors.Add($"This tourney is fenced to {maxScore}; a score cannot go above that");
                return resultModel;
            }

            if (!await TryTakeClaimAsync(matchId, judgeName))
            {
                resultModel.Errors.Add($"{match.ClaimedBy} is scoring this match right now");
                return resultModel;
            }

            await _dbContext.Entry(match).ReloadAsync();

            match.FirstScore = firstScore;
            match.SecondScore = secondScore;
            match.Status = finish ? MatchStatus.Finished : MatchStatus.InProgress;
            match.UpdatedAt = DateTime.UtcNow;

            if (finish)
            {
                match.ClaimedBy = null;
                match.ClaimExpiresAt = null;
            }

            await _dbContext.SaveChangesAsync();

            resultModel.Data = match;
            return resultModel;
        }

        public async Task<ResultModel<Match>> ReopenAsync(Guid matchId)
        {
            var resultModel = new ResultModel<Match>();

            var match = await WithPlayers().FirstOrDefaultAsync(m => m.Id == matchId);

            if (match is null)
            {
                resultModel.Errors.Add("Match does not exist");
                return resultModel;
            }

            if (match.Status != MatchStatus.Finished)
            {
                resultModel.Errors.Add("Only a finished match can be reopened");
                return resultModel;
            }

            match.Status = MatchStatus.InProgress;
            match.UpdatedAt = DateTime.UtcNow;
            match.ClaimedBy = null;
            match.ClaimExpiresAt = null;

            await _dbContext.SaveChangesAsync();

            resultModel.Data = match;
            return resultModel;
        }

        public async Task<ResultModel<Match>> DeleteAsync(Match entity)
        {
            _dbContext.Matches.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return new ResultModel<Match> { Data = entity };
        }


    }
}