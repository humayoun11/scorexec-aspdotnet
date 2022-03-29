using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Grounds.Dto
{
    public class GroundListDto : EntityDto, IHasCreationTime
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
