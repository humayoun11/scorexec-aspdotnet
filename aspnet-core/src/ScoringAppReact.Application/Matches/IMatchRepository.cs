using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ScoringAppReact.Matches.Dto;
using ScoringAppReact.Models;

namespace ScoringAppReact.Matches
{
    public interface IMatchRepository 
    {
        Task<IQueryable> GetMatchesByEventId(long id, int tenantId);
    }
}
