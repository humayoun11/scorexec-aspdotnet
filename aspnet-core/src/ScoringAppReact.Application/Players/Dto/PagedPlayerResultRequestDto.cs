using Abp.Application.Services.Dto;

namespace ScoringAppReact.Players.Dto
{
    public class PagedPlayerResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public long? TeamId { get; set; }
        public int? BattingStyle { get; set; }
        public int? BowlingStyle { get; set; }
        public int? PlayingRole { get; set; }
        public string Contact { get; set; }
    }
}

