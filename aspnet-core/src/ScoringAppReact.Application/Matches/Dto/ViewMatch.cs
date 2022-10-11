using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Matches.Dto
{
    public class ViewMatch
    {
        public long Id { get; set; }
        public long? EventId { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public long Team1Id { get; set; }
        public long Team2Id { get; set; }
        public long? Team1Score { get; set; }
        public float? Team1Overs { get; set; }
        public long? Team1Wickets { get; set; }
        public long? Team2Wickets { get; set; }
        public float? Team2Overs { get; set; }
        public long? Team2Score { get; set; }
        public string MatchType { get; set; }
        public string Tournament { get; set; }
        public string Mom { get; set; }
        public string Ground { get; set; }
        public double? Date { get; set; }
        public string Result { get; set; }
        public long? POM { get; set; }
    }
}
