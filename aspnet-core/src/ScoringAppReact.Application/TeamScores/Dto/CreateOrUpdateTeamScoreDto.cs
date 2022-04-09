using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.TeamScores.Dto
{
    public class CreateOrUpdateTeamScoreDto
    {
        public long? Id { get; set; }
        public int TotalScore { get; set; }
        public int Overs { get; set; }
        public int Wickets { get; set; }
        public int? Wideballs { get; set; }
        public int? NoBalls { get; set; }
        public int? Byes { get; set; }
        public int? LegByes { get; set; }
        public long TeamId { get; set; }
        public long MatchId { get; set; }
    }
}
