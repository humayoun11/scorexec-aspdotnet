using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Events.Dto
{
    public class EventListDto : EntityDto, IHasCreationTime
    {
        public string Name { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
        public int EventType { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
