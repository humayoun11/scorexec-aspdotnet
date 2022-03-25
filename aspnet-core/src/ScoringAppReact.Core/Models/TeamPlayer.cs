using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ScoringAppReact.Models
{
    public class TeamPlayer : CustomAuditableEntity
    {
        public long TeamId { get; set; }
        public long PlayerId { get; set; }

        [ForeignKey("PlayerId")]
        public Player Player { get; set; }

        [ForeignKey("TeamId")]
        public Team Team { get; set; }
    }
}
