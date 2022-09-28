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

        public async Task<List<PlayerScore>> GetAll(long? teamId, long? matchId, long? player1Id, long? player2Id, int? tenantId)
        {
            var result = await _repository.GetAll().
                Include(i => i.Player).
                Where(i => i.IsDeleted == false &&
                (!teamId.HasValue || i.TeamId == teamId) &&
                (!matchId.HasValue || i.MatchId == matchId) &&
                (!tenantId.HasValue || i.TenantId == tenantId) &&
                (!player1Id.HasValue || i.PlayerId == player1Id || !player2Id.HasValue || i.PlayerId == player2Id))
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


        public async Task<List<PlayerScore>> GetAllPlayers(long? matchId, long? eventId, int? tenantId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false &&
                (!tenantId.HasValue || i.TenantId == tenantId) &&
                (!matchId.HasValue || i.MatchId == matchId) &&
                (!eventId.HasValue || i.Match.EventId == eventId))
                .ToListAsync();
            return result;
        }


    }
}
