using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.MatchSummary.Dto
{
    public class TeamsScoreDto
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public int? Score { get; set; }
        public float? Overs { get; set; }
        public int? Wickets { get; set; }
        public int? Wide { get; set; }
        public int? NoBall { get; set; }
        public int? LegBye { get; set; }
        public int? Bye { get; set; }
        public int? Extras { get; set; }
    }
}
