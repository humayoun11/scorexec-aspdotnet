using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class FallOfWicket : FullAuditedEntity<long>, IMayHaveTenant
    {
        public long MatchId { get; set; }
        public long TeamId { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        public int Third { get; set; }
        public int Fourth { get; set; }
        public int Fifth { get; set; }
        public int Sixth { get; set; }
        public int Seventh { get; set; }
        public int Eight { get; set; }
        public int Ninth { get; set; }
        public int Tenth { get; set; }
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
        [ForeignKey("MatchId")]
        public Match Match { get; set; }
        public int? TenantId { get; set; }
    }
}