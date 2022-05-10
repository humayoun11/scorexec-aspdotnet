using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Teams.Dto
{
    public class TeamStatsDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int? Type { get; set; }
        public int? Matches { get; set; }
        public int? Won { get; set; }
        public int? Lost { get; set; }
        public int? Tie { get; set; }
        public int? Drawn { get; set; }
        public int? NoResult { get; set; }
        public int? TossWon { get; set; }
        public int? BatFirst { get; set; }
        public int? FieldFirst { get; set; }
    }
}
