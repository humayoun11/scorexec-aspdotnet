using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Players.Dto;

namespace ScoringAppReact.Players
{
    public interface IPlayerAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdatePlayerDto typeDto);

        Task<PlayerDto> GetById(long typeId);

        Task<ResponseMessageDto> DeleteAsync(long typeId);

        Task<List<PlayerDto>> GetAll(long? tenantId);

        Task<PagedResultDto<PlayerDto>> GetPaginatedAllAsync(PagedPlayerResultRequestDto input);
    }
}
