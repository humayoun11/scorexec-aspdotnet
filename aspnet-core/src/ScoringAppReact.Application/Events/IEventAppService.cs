using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Events.Dto;
namespace ScoringAppReact.Events
{
    public interface IEventAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateEventDto typeDto);

        Task<EventDto> GetById(long typeId);

        Task<ResponseMessageDto> DeleteAsync(long typeId);

        Task<List<EventDto>> GetAll(long? tenantId);

        Task<PagedResultDto<EventDto>> GetPaginatedAllAsync(PagedEventResultRequestDto input);
    }
}
