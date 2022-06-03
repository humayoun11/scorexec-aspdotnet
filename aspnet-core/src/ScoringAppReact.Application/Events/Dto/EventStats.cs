using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Models;

namespace ScoringAppReact.Events.Dto
{
    public class EventStats
    {
        public string Event { get; set; }
        public string Organizor { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
        public int? Matches { get; set; }
        public string Batsman { get; set; }
        public string Bowler { get; set; }
        public string Pot { get; set; }
        public int? Catches { get; set; }
        public int? Sixes { get; set; }
        public int? Fours { get; set; }
        public int? Wickets { get; set; }
        public int? Stumps { get; set; }
        public int? Runouts { get; set; }
        public int? Groups { get; set; }
    }
}