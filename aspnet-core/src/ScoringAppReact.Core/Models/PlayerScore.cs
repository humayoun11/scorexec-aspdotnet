using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Models
{
    public class PlayerScore : FullAuditedEntity<long>, IMayHaveTenant
    {
        public long PlayerId { get; set; }
        public int Position { get; set; }
        public long MatchId { get; set; }
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
        public long TeamId { get; set; }
        public bool IsPlayedInning { get; set; }
        public Player Bowler { get; set; }
        public Player Player { get; set; }
        public Match Match { get; set; }
        public Team Team { get; set; }
        public int? TenantId { get; set; }
    }
}