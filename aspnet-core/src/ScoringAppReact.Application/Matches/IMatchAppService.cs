using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Matches.Dto;

namespace ScoringAppReact.Matches
{
    public interface IMatchAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateMatchDto typeDto);

        Task<CreateOrUpdateMatchDto> GetById(long typeId);

        Task<ResponseMessageDto> DeleteAsync(long typeId);

        Task<List<MatchDto>> GetAll(long? tenantId);

        Task<PagedResultDto<MatchDto>> GetPaginatedAllAsync(PagedMatchResultRequestDto input);
    }
}
