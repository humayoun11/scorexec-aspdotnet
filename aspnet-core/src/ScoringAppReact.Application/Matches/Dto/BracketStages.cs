using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Matches.Dto
{
    public class BracketStages
    {
        public List<EventMatches> EventMatches { get; set; }
        public TeamDto Winner { get; set; }
    }
}
