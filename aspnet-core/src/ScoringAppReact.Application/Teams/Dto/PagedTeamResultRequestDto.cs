using Abp.Application.Services.Dto;

namespace ScoringAppReact.Teams.Dto
{
    public class PagedTeamResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public long? TenantId { get; set; }
    }
}

