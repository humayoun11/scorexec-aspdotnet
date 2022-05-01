using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Matches.Dto
{
    public class StageTeams
    {
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public long? Date { get; set; }
    }
}
