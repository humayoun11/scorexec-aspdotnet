using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Events.Dto;
namespace ScoringAppReact.Events
{
    public interface IEventAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateEventDto model);

        Task<EventDto> GetById(long id);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<EventDto>> GetAll();

        Task<PagedResultDto<EventDto>> GetPaginatedAllAsync(PagedEventResultRequestDto input);

        Task<ResponseMessageDto> CreateOrUpdateEventTeamsAsync(EventTeamDto model);

        Task<List<EventDto>> GetAllEventsByTeamId(long id, int? typeId);

        Task<EventStats> GetEventStat(long id);
    }
}
