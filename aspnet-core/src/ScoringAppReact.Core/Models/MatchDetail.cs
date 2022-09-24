using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class MatchDetail : FullAuditedEntity<long>
    {
        public int Status { get; set; }
        public int? Inning { get; set; }
        public bool IsLiveStreaming { get; set; }
        public int ScoringBy { get; set; }
        public long MatchId { get; set; }
        [ForeignKey("MatchId")]
        public Match Match { get; set; }
    }
}