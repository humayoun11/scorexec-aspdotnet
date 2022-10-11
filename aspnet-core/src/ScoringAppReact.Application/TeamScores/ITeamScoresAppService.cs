using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.MatchSummary.Dto;
using ScoringAppReact.Players.Dto;
using ScoringAppReact.TeamScores.Dto;

namespace ScoringAppReact.TeamScores
{
    public interface ITeamScoresAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateTeamScoreDto model);

        Task<TeamScoreDto> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<TeamScoreDto> GetByTeamIdAndMatchId(long teamId, long matchId);

        Task<MatchDetails> GetTeamScorecard(long team1Id, long team2Id, long matchId);

        Task<FullScoreccard> GetFullScorecard(long team1Id, long team2Id, long matchId);
    }
}

