using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Matches.Dto
{
    public class BracketStages
    {
        public List<StageTeams> StageTeams { get; set; }
    }
}
