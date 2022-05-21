using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.MatchSummary.Dto
{
    public class TeamScoreDto
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public int? Score { get; set; }
        public float? Overs { get; set; }
    }
}
