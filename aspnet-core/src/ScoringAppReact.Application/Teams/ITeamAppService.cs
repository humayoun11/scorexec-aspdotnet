using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Teams
{
    public interface ITeamAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateTeamDto typeDto);

        Task<TeamDto> GetById(long typeId);

        Task<ResponseMessageDto> DeleteAsync(long typeId);

        Task<List<TeamListDto>> GetAll();

        Task<PagedResultDto<TeamDto>> GetPaginatedAllAsync(PagedTeamResultRequestDto input);

        Task<List<TeamDto>> GetAllTeamsByEventId(long id);

        Task<List<TeamDto>> GetAllTeamsByMatchId(long id);

        Task<List<TeamDto>> GetAllTeamsByPlayerId(long id);

        Task<TeamStatsDto> GetTeamStats(long id);
    }
}
