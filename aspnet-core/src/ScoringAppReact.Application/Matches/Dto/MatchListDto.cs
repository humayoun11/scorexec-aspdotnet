using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Matches.Dto
{
    public class MatchListDto : EntityDto, IHasCreationTime
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
