using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Players.Dto;

namespace ScoringAppReact.Players
{
    public interface IPlayerAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdatePlayerDto model);

        Task<PlayerDto> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<PlayerDto>> GetAll();

        Task<PagedResultDto<PlayerDto>> GetPaginatedAllAsync(PagedPlayerResultRequestDto input);

        Task<List<PlayerDto>> GetAllByTeamId(long teamId);

        Task<PlayerStatisticsDto> PlayerStatistics(int id);
    }
}
