using Abp.Application.Services.Dto;

namespace ScoringAppReact.Events.Dto
{
    public class PagedEventResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public long? TenantId { get; set; }
    }
}

