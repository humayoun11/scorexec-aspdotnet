using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Players.Dto;
using ScoringAppReact.Statistics.Dto;

namespace ScoringAppReact.Statistics
{
    public interface IStatisticsAppService : IApplicationService
    {
        Task<List<MostRunsdto>> MostRuns(MostRunsInput input);
    }
}
