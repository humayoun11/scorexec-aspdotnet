using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Teams.Dto
{
    public class TeamListDto : EntityDto, IHasCreationTime
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
