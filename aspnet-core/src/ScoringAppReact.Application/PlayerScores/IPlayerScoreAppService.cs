using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ScoringAppReact.PlayerScores.Dto;

namespace ScoringAppReact.PlayerScores
{
    public interface IPlayerScoreAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdatePlayerScoreDto model);

        Task<PlayerScoreDto> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<PlayerScoreListDto>> GetAll(long teamId, long matchId);

        Task<ResponseMessageDto> CreatePlayerScoreListAsync(List<CreateOrUpdatePlayerScoreDto> model);
    }
}

