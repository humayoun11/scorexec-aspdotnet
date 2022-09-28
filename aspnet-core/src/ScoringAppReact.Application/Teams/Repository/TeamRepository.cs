using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Teams.Repository
{
    public class TeamRepository : ITeamRepository
    {

        private readonly IRepository<Team, long> _repository;
        public TeamRepository(IRepository<Team, long> repository)
        {
            _repository = repository;
        }

        public async Task<Team> Get(long id)
        {
            var result = await _repository.GetAll()
                .Where(i => i.Id == id && i.IsDeleted == false)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<Team>> GetAll(int? tenantId)
        {
            return await _repository.GetAll()
                .Include(i=> i.EventTeams)
                .Where(i => i.IsDeleted == false && (!tenantId.HasValue || i.TenantId == tenantId))
                .ToListAsync();
        }
    }
}
