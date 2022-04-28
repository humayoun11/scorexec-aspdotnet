using Abp.Application.Services.Dto;

namespace ScoringAppReact.Events.Dto
{
    public class PagedEventResultRequestDto : PagedResultRequestDto
    {
        public string Name { get; set; }
        public int? Type { get; set; }
        public int? StartDate { get; set; }
        public int? EndDate { get; set; }
    }
}

