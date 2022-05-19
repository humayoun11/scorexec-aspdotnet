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
        Task<List<MostRunsdto>> MostRuns(BattingInput input);

        Task<List<MostCenturiesdto>> MostCenturies(BattingInput input);

        Task<List<MostFiftiesdto>> MostFifties(BattingInput input);

        Task<List<MostFoursdto>> MostFours(BattingInput input);

        Task<List<MostSixesdto>> MostSixes(BattingInput input);

        Task<List<MostWicketsdto>> MostWickets(BowlingInput input);

        Task<List<MostCatchesdto>> MostCatches(BowlingInput input);

        Task<List<MostStumpsdto>> MostStumps(BowlingInput input);

        Task<List<MostMaidensdto>> MostMaidens(BowlingInput input);

        Task<List<MostRunoutsdto>> MostRunouts(BowlingInput input);

        Task<List<HighestWicket>> HighestWicketsInAnInning(BowlingInput input);

        Task<List<BestScore>> HighestRunsInAnInning(BowlingInput input);
    }
}
