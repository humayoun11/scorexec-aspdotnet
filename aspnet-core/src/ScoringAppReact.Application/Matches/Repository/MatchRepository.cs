using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Extensions;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Matches.Repository
{
    public class MatchRepository : IMatchRepository
    {
        private readonly IRepository<Match, long> _repository;

        public MatchRepository(IRepository<Match, long> repository)
        {
            _repository = repository;
        }

        public Task<IQueryable> GetMatchesByEventId(long id, int tenantId)
        {
            var result = _repository.GetAll()
                .Where(i => i.IsDeleted == false
                && i.TenantId == tenantId
                && i.EventId == id).AsQueryable();
            return (Task<IQueryable>)result;
        }

        public async Task<Match> GetMatchThen(long matchId)
        {
            return await _repository.GetAll()
                   .Include(i => i.MatchDetail)
                   .Include(i => i.TeamScores).ThenInclude(j => j.Team)
                   .Where(i => i.Id == matchId && i.IsDeleted == false)
                   .FirstOrDefaultAsync();
        }

        public async Task<Match> GetAll(long? matchId, bool detailInclude = false, bool scoreInclude = false, bool groundInclude = false, bool eventInclude = false)
        {
            return await _repository.GetAll()
                   .IncludeIf(detailInclude, i => i.MatchDetail)
                   .IncludeIf(scoreInclude, i => i.TeamScores)
                   .IncludeIf(groundInclude, i => i.Ground)
                   .IncludeIf(eventInclude, i => i.Event)
                   .Where(i => (!matchId.HasValue || i.Id == matchId) && i.IsDeleted == false)
                   .FirstOrDefaultAsync();
        }

        public void InsertOrUpdateRange(List<Match> models)
        {
            _repository.GetDbContext().UpdateRange(models);
        }

        public async Task<Match> Update(Match model)
        {
            return await _repository.UpdateAsync(model);

        }

        public async Task<Match> Insert(Match model)
        {
            return await _repository.InsertAsync(model);

        }
    }
}
