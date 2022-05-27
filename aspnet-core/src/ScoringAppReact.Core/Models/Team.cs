using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScoringAppReact.Models
{
    public class Team : FullAuditedEntity<long>, IMayHaveTenant
    {
        public Team()
        {
            OpponentTeamMatches = new List<Match>();
            TeamPlayers = new List<TeamPlayer>();
            HomeTeamMatches = new List<Match>();
            TeamScores = new List<TeamScore>();
            FallOfWickets = new List<FallOfWicket>();
            MatchSchedules = new List<MatchSchedule>();
        }

        [Required]
        public string Name { get; set; }
        [StringLength(100)]
        public string Place { get; set; }
        public string Zone { get; set; }
        public string Contact { get; set; }
        public int Type { get; set; }
        public bool IsRegistered { get; set; }
        [Required]
        public string City { get; set; }
        public string FileName { get; set; }
        public List<Match> OpponentTeamMatches { get; set; }
        public List<Match> HomeTeamMatches { get; set; }
        public List<TeamScore> TeamScores { get; set; }
        public List<FallOfWicket> FallOfWickets { get; set; }
        public List<MatchSchedule> MatchSchedules { get; set; }
        public List<EventTeam> EventTeams { get; set; }
        public List<TeamPlayer> TeamPlayers { get; set; }
        public bool? IsVerified { get; set; }
        public int? TenantId { get; set; }
    }
}