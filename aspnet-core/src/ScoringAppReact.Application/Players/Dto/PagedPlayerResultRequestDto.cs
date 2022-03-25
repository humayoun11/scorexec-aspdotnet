using Abp.Application.Services.Dto;

namespace ScoringAppReact.Players.Dto
{
    public class PagedPlayerResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public long? TenantId { get; set; }
    }
}

