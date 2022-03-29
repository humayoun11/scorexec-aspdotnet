using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Grounds.Dto;

namespace ScoringAppReact.Grounds
{
    public interface IGroundAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateGroundDto typeDto);

        Task<GroundDto> GetById(long typeId);

        Task<ResponseMessageDto> DeleteAsync(long id);

        Task<List<GroundDto>> GetAll();

        Task<PagedResultDto<GroundDto>> GetPaginatedAllAsync(PagedGroundResultRequestDto input);
    }
}
