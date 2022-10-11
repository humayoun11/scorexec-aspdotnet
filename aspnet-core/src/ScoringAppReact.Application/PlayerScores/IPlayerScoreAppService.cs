using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ScoringAppReact.Models;
using ScoringAppReact.PlayerScores.Dto;

namespace ScoringAppReact.PlayerScores
{
    public interface IPlayerScoreAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdatePlayerScoreDto model);

        Task<PlayerScore> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<PlayerScoreListDto>> GetAll(long? teamId, long matchId);

        Task<ResponseMessageDto> CreatePlayerScoreListAsync(List<CreateOrUpdatePlayerScoreDto> model);
    }
}

