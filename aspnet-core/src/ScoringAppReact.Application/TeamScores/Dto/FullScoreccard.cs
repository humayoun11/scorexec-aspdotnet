using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.FallOfWickets.Dto;
using ScoringAppReact.PlayerScores.Dto;

namespace ScoringAppReact.TeamScores.Dto
{
    public class FullScoreccard
    {
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public List<PlayerScoreListDto> Team1Playerscore { get; set; }
        public List<PlayerScoreListDto> Team2Playerscore { get; set; }
        public List<FallofWicketsDto> Team1FallofWicket { get; set; }
        public List<FallofWicketsDto> Team2FallofWicket { get; set; }
        public TeamScoreDto Team1Score { get; set; }
        public TeamScoreDto Team2Score { get; set; }
    }
}
