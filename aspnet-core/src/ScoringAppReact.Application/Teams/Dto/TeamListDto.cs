using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Teams.Dto
{
    public class TeamListDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long EventId { get; set; }
    }
}
