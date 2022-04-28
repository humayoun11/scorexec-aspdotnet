using Abp.Application.Services.Dto;

namespace ScoringAppReact.Matches.Dto
{
    public class PagedMatchResultRequestDto : PagedResultRequestDto
    {
        public long? GroundId { get; set; }
        public long? Date { get; set; }
        public long? Team1Id { get; set; }
        public long? Team2Id { get; set; }
        public int? Overs { get; set; }
        public int? Type { get; set; }
    }
}

