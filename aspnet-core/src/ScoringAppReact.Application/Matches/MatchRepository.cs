using Abp.Domain.Repositories;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Matches
{
    public class MatchRepository
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
    }
}
