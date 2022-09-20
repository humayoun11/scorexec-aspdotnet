using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ScoringAppReact.Authorization.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class EntityAdmin : FullAuditedEntity<long>, IMayHaveTenant
    {
        public long UserId { get; set; }
        public long? MatchId { get; set; }
        public long? TeamId { get; set; }
        public long? EventId { get; set; }
        public long? GroundId { get; set; }
        public long? PlayerId { get; set; }
        [ForeignKey("MatchId")]
        public Match Match { get; set; }
        [ForeignKey("MatchId")]
        public Player Player { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [ForeignKey("GroundId")]
        public Ground Ground { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public int? TenantId { get; set; }
    }
}