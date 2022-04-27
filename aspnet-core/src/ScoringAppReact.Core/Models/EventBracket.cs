using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class EventBracket : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public int NoOfTeams { get; set; }
        public int NoOfGroups { get; set; }
        public long EventId { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }
    }
}