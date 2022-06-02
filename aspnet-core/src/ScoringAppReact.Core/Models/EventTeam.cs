using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class EventTeam : FullAuditedEntity<long>, IMayHaveTenant
    {

        public int? Group { get; set; }
        public long EventId { get; set; }
        public long TeamId { get; set; }
        [ForeignKey("TeamId")]
        public Team Team { get; set; }
        [ForeignKey("EventId")]
        public Event Event { get; set; }
        public int? TenantId { get; set; }
    }
}