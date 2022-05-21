using System.Threading.Tasks;
using Abp.Application.Services;
using ScoringAppReact.MatchSummary.Dto;

namespace ScoringAppReact.MatchSummary
{
    public interface IMatchSummaryAppService : IApplicationService
    {
        Task<MatchDetails> GetTeamScorecard(long team1Id, long team2Id, long matchId);
    }
}

