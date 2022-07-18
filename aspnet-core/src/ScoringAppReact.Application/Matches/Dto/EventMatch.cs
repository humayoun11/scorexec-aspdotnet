using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Matches.Dto
{
    public class EventMatch
    {
        public long? Id { get; set; }
        public long? GroundId { get; set; }
        public string Ground { get; set; }
        public int MatchOvers { get; set; }
        public string MatchDescription { get; set; }
        public int? Season { get; set; }
        public long? EventId { get; set; }
        public string Event { get; set; }
        public long? TossWinningTeam { get; set; }
        public long? DateOfMatch { get; set; }
        public long Team1Id { get; set; }
        public string Team1{ get; set; }
        public long Team2Id { get; set; }
        public string Team2 { get; set; }
        public int MatchTypeId { get; set; }
        public int? EventStage { get; set; }
        public long? PlayerOTM { get; set; }
        public List<GalleryDto> Pictures { get; set; }
        public string ProfileUrl { get; set; }
    }
}
