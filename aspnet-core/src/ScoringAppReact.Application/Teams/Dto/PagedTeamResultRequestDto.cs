using Abp.Application.Services.Dto;

namespace ScoringAppReact.Teams.Dto
{
    public class PagedTeamResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public int? Type { get; set; }
    }
}

