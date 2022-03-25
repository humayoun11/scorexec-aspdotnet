using Abp.Application.Services.Dto;

namespace ScoringAppReact.Matches.Dto
{
    public class PagedMatchResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public long? GroundId { get; set; }
        public int MatchOvers { get; set; }
        public int? Season { get; set; }
        public long? EventId { get; set; }
        public long? TossWinningTeam { get; set; }
        public long? DateOfMatch { get; set; }
        public long HomeTeamId { get; set; }
        public long OppponentTeamId { get; set; }
        public float Overs { get; set; }
        public int MatchTypeId { get; set; }
        public int? EventStage { get; set; }
        public long? PlayerOTM { get; set; }
        public long? TenantId { get; set; }
    }
}

