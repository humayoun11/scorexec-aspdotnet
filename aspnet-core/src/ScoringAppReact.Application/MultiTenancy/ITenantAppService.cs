using Abp.Application.Services;
using ScoringAppReact.MultiTenancy.Dto;

namespace ScoringAppReact.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

