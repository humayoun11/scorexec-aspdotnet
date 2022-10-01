using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Matches.Dto;
using ScoringAppReact.Models;

namespace ScoringAppReact.Matches.Repository
{
    public interface IMatchRepository
    {
        Task<IQueryable> GetMatchesByEventId(long id, int tenantId);

        Task<Match> GetMatchThen(long matchId);

        Task<Match> GetAll(long? matchId, bool detailInclude = false, bool scoreInclude = false, bool groundInclude = false, bool eventInclude = false);

        void InsertOrUpdateRange(List<Match> models);

        Task<Match> Insert(Match model);

        Task<Match> Update(Match model);
    }
}
