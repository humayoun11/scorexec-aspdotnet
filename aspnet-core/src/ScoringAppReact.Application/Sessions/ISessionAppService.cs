using System.Threading.Tasks;
using Abp.Application.Services;
using ScoringAppReact.Sessions.Dto;

namespace ScoringAppReact.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
