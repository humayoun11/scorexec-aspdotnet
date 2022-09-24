using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.PlayerScores.Dto
{
    public class CreateOrUpdatePlayerScoreDto
    {
        public long? Id { get; set; }
        public long PlayerId { get; set; }
        public bool IsStriker { get; set; }
        public bool IsBowling { get; set; }
        public int? Position { get; set; }
        public long MatchId { get; set; }
        public long TeamId { get; set; }
        public long? BowlerId { get; set; }
        public int? Bat_Runs { get; set; }
        public int? Bat_Balls { get; set; }
        public int? HowOutId { get; set; }
        public int? Ball_Runs { get; set; }
        public float? Overs { get; set; }
        public int? Wickets { get; set; }
        public int? Stump { get; set; }
        public int? Catches { get; set; }
        public int? Maiden { get; set; }
        public int? RunOut { get; set; }
        public int? Four { get; set; }
        public int? Six { get; set; }
        public string Fielder { get; set; }
        public bool IsPlayedInning { get; set; }
    }
}
