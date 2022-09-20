using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using ScoringAppReact.PlayerScores.Dto;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Matches.Dto
{
    public class MatchDetailAndPlayerListDto
    {
        public long? Id { get; set; }
        public int Status { get; set; }
        public bool IsLiveStreaming { get; set; }
        public int ScoringBy { get; set; }
        public long MatchId { get; set; }
        public List<CreateOrUpdatePlayerScoreDto> Players { get; set; }
    }
}
