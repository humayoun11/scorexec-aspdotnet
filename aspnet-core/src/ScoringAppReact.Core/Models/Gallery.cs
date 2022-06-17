using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class Gallery : FullAuditedEntity<long>
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public long? PlayerId { get; set; }
        public long? MatchId { get; set; }
        public long? TeamId { get; set; }
        public long? GroundId { get; set; }
        public long? EventId { get; set; }

        [ForeignKey("PlayerId")]
        public Player Player { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }

        [ForeignKey("MatchId")]
        public Match Match { get; set; }

        [ForeignKey("GroundId")]
        public Ground Ground { get; set; }
    }
}