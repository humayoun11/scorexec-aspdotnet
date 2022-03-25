using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Models
{
    public class TeamScore : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int TotalScore { get; set; }
        public int Overs { get; set; }
        public int Wickets { get; set; }
        public int Wideballs { get; set; }
        public int NoBalls { get; set; }
        public int Byes { get; set; }
        public int LegByes { get; set; }
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public long MatchId { get; set; }
        public Match Match { get; set; }
        public int? TenantId { get; set; }
    }
}