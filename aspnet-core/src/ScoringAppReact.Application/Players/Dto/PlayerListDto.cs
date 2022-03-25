using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Players.Dto
{
    public class PlayerListDto : EntityDto, IHasCreationTime
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
