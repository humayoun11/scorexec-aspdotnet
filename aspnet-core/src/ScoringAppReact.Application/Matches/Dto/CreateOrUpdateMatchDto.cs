using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Matches.Dto
{
    public class CreateOrUpdateMatchDto
    {
        public long? Id { get; set; }
        public long? GroundId { get; set; }
        public int MatchOvers { get; set; }
        public string MatchDescription { get; set; }
        public int? Season { get; set; }
        public long? EventId { get; set; }
        public long? TossWinningTeam { get; set; }
        public long? DateOfMatch { get; set; }
        public long Team1_Id { get; set; }
        public long Team2_Id { get; set; }
        public string FileName { get; set; }
        public int MatchTypeId { get; set; }
        public int? EventStage { get; set; }
        public long? PlayerOTM { get; set; }
        public int? TenantId { get; set; }
    }
}
