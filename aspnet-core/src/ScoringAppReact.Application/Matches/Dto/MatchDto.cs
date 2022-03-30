using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Models;

namespace ScoringAppReact.Matches.Dto
{
    public class MatchDto : EntityDto<long>
    {
        public string Ground { get; set; }
        public int MatchOvers { get; set; }
        public string MatchDescription { get; set; }
        public int? Season { get; set; }
        public long? EventId { get; set; }
        public string TossWinningTeam { get; set; }
        public long? DateOfMatch { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string FileName { get; set; }
        public string MatchType { get; set; }
        public string EventStage { get; set; }
        public string PlayerOTM { get; set; }
        public int? TenantId { get; set; }
    }
}