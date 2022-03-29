using Abp.Application.Services.Dto;

namespace ScoringAppReact.Grounds.Dto
{
    public class PagedGroundResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public long? TenantId { get; set; }
    }
}

