using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Players.Repository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IRepository<Player, long> _repository;
        public PlayerRepository(IRepository<Player, long> repository)
        {
            _repository = repository;
        }


        public async Task<List<Player>> GetAll(int? tenantId, long? teamId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false &&
                (!tenantId.HasValue || i.TenantId == tenantId) &&
                (!teamId.HasValue || i.Teams.Any(j => j.TeamId == teamId))
                )
                .ToListAsync();
            return result;
        }


        public async Task<Player> Get(long id)
        {
            var result = await _repository.GetAll().Where(i => i.Id == id && i.IsDeleted == false).Include(i => i.Pictures).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<Player>> GetAllByTeamIds(List<long> teamIds, int? tenantId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false &&
                (!tenantId.HasValue || i.TenantId == tenantId) && i.Teams.Any(j => teamIds.Contains(j.TeamId))).ToListAsync();
            return result;
        }
    }
}
