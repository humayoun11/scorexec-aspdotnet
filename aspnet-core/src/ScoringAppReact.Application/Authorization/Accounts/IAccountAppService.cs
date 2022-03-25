using System.Threading.Tasks;
using Abp.Application.Services;
using ScoringAppReact.Authorization.Accounts.Dto;

namespace ScoringAppReact.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
