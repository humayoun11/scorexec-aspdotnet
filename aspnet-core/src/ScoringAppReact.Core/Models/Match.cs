using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScoringAppReact.Models
{
    public class Match : FullAuditedEntity<long>, IMayHaveTenant
    {
        public Match()
        {
            PlayerScores = new List<PlayerScore>();
            TeamScores = new List<TeamScore>();
            FallOfWickets = new List<FallOfWicket>();
        }
        public long? GroundId { get; set; }
        [Required]
        public int MatchOvers { get; set; }
        public string MatchDescription { get; set; }
        public int? Season { get; set; }
        public long? EventId { get; set; }
        public long? TossWinningTeam { get; set; }
        //public long? MatchWinningTeam { get; set; }
        public long? DateOfMatch { get; set; }
        public long HomeTeamId { get; set; }
        public long OppponentTeamId { get; set; }
        public string ProfileUrl { get; set; }
        public int MatchTypeId { get; set; }
        public int? EventStage { get; set; }
        public long? PlayerOTM { get; set; }

        [ForeignKey("PlayerOTM")]
        public Player Player { get; set; }

        [ForeignKey("GroundId")]
        public Ground Ground { get; set; }

        [ForeignKey("OppponentTeamId")]
        public Team OppponentTeam { get; set; }

        [ForeignKey("HomeTeamId")]
        public Team HomeTeam { get; set; }

        [ForeignKey("EventId")]
        public Event Event { get; set; }
        public List<Gallery> Pictures { get; set; }
        public List<PlayerScore> PlayerScores { get; set; }
        public List<TeamScore> TeamScores { get; set; }
        public List<FallOfWicket> FallOfWickets { get; set; }
        public int? TenantId { get; set; }
    }
}