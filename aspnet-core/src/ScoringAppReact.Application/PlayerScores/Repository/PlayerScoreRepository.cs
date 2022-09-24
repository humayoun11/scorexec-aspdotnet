using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.PlayerScores.Repository
{
    public class PlayerScoreRepository : IPlayerScoreRepository
    {
        private readonly IRepository<PlayerScore, long> _repository;

        public PlayerScoreRepository(IRepository<PlayerScore, long> repository)
        {
            _repository = repository;
        }

        public async Task<List<PlayerScore>> GetAll(long? teamId, long matchId, int? tenantId)
        {
            var result = await _repository.GetAll().
                Include(i => i.Player).
                Where(i => i.IsDeleted == false &&
                (!teamId.HasValue || i.TeamId == teamId) &&
                (!tenantId.HasValue || i.TenantId == tenantId) &&
                i.MatchId == matchId)
                .ToListAsync();
            return result;
        }

        public async Task<PlayerScore> Get(long id)
        {
            var result = await _repository.GetAll().
                Where(i => i.IsDeleted == false &&
                i.Id == id)
                .FirstOrDefaultAsync();
            return result;
        }

    }
}
