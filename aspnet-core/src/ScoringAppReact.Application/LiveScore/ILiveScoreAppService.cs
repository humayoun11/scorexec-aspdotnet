using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ScoringAppReact.LiveScore.Dto;
using ScoringAppReact.Models;
using ScoringAppReact.PlayerScores.Dto;

namespace ScoringAppReact.LiveScore
{
    public interface ILiveScoreAppService : IApplicationService
    {
        Task<LiveScoreDto> Get(long matchId, bool newOver = false);
        Task<List<PlayerScore>> GetPlayers(long matchId, long teamId);
        Task<LiveScoreDto> Submit(InputLiveScoreDto model);

        Task<LiveScoreDto> ChangeBowler(InputChangeBowler model);
    }
}

