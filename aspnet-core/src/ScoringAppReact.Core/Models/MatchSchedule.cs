using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Models
{
    public class MatchSchedule:  FullAuditedEntity<long>
    {
        public string GroundName { get; set; }
        public string OpponentTeam { get; set; }
        public string Month { get; set; }
        public int? Day { get; set; }
        public int? Year { get; set; }
        public long TeamId { get; set; }
        public Team Team { get; set; }
        public int? TenantId { get; set; }
    }
}