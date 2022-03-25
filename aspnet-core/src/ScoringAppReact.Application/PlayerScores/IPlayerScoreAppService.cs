using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Players.Dto;

namespace ScoringAppReact.PlayerScores
{
    public interface IPlayerScoreAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdatePlayerScoreDto typeDto);

        Task<PlayerScoreDto> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<PlayerScoreDto>> GetAll(long teamId, long matchId);
    }
}

