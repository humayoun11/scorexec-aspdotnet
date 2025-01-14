using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Models;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Matches.Dto
{
    public class MatchDto : EntityDto<long>
    {
        public string Ground { get; set; }
        public long? GroundId { get; set; }
        public long MatchTypeId { get; set; }
        public int MatchOvers { get; set; }
        public string MatchDescription { get; set; }
        public int? Season { get; set; }
        public long? EventId { get; set; }
        public string EventName { get; set; }
        public long? TossWinningTeam { get; set; }
        public long? DateOfMatch { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string Team1ProfileUrl { get; set; }
        public string Team2ProfileUrl { get; set; }
        public long Team1Id { get; set; }
        public long Team2Id { get; set; }
        public string FileName { get; set; }
        public string MatchType { get; set; }
        public string EventStage { get; set; }
        public int? EventStageId { get; set; }
        public long? PlayerOTM { get; set; }
        public List<GalleryDto> Pictures { get; set; }
        public int? TenantId { get; set; }
        public string ProfileUrl { get; set; }
    }
}