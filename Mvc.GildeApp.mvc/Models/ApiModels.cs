using System;
using System.Collections.Generic;

namespace Mvc.GildeApp.mvc.Models
{
    /// <summary>
    /// The API wraps every payload in { data, errors, isSuccess }.
    /// </summary>
    public class ApiResult<T>
    {
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool IsSuccess { get; set; }
    }

    public class TourneyModel
    {
        public Guid TourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string WeaponName { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public int MatchesTotal { get; set; }
        public int MatchesFinished { get; set; }
    }

    public class TourneyDetailModel : TourneyModel
    {
        public Guid RuleSetId { get; set; }
        public RuleSetModel? RuleSet { get; set; }
        public List<TourneyEntryModel> Entries { get; set; } = new();
    }

    public class RuleSetModel
    {
        public Guid RuleSetId { get; set; }
        public Guid WeaponId { get; set; }
        public int MaxScore { get; set; }
        public int Doubles { get; set; }
        public bool HasDoubles { get; set; }
        public WeaponModel? Weapon { get; set; }
    }

    public class WeaponModel
    {
        public Guid WeaponId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PlayerModel
    {
        public Guid PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class TourneyEntryModel
    {
        public Guid EntryId { get; set; }
        public Guid TourneyId { get; set; }
        public Guid PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Position { get; set; }
    }

    public class BoardModel
    {
        public Guid TourneyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string WeaponName { get; set; } = string.Empty;
        public int MaxScore { get; set; }
        public int PlayerCount { get; set; }
        public int MatchesTotal { get; set; }
        public int MatchesFinished { get; set; }
        public List<BoardRowModel> Rows { get; set; } = new();
        public List<BoardMatchModel> Matches { get; set; } = new();
    }

    public class BoardRowModel
    {
        public Guid EntryId { get; set; }
        public Guid PlayerId { get; set; }
        public int Position { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public int TotalReceived { get; set; }
        public int MatchesPlayed { get; set; }
        public int Placement { get; set; }
        public List<BoardCellModel> Cells { get; set; } = new();
    }

    public class BoardCellModel
    {
        public int OpponentPosition { get; set; }
        public bool IsSelf { get; set; }
        public Guid? MatchId { get; set; }
        public int? Score { get; set; }
        public int? OpponentScore { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class BoardMatchModel
    {
        public Guid MatchId { get; set; }
        public int Order { get; set; }
        public string Status { get; set; } = string.Empty;
        public int FirstPosition { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public int FirstScore { get; set; }
        public int SecondPosition { get; set; }
        public string SecondName { get; set; } = string.Empty;
        public int SecondScore { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
