using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Players.Dto;
using ScoringAppReact.TeamScores.Dto;

namespace ScoringAppReact.TeamScores
{
    public interface ITeamScoresAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateTeamScoreDto model);

        Task<TeamScoreDto> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<TeamScoreDto>> GetAll(long teamId, long matchId);
    }
}

