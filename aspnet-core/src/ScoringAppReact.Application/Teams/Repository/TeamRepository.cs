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
                .Include(i => i.Pictures)
                .Where(i => i.Id == id && i.IsDeleted == false)
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<Team>> GetAll(int? tenantId, long? eventId, bool eventTeamInclude = false)
        {
            var result = await _repository.GetAll()
                .IncludeIf(eventTeamInclude, i => i.EventTeams)
                .Where(i => i.IsDeleted == false && (!tenantId.HasValue || i.TenantId == tenantId) &&
                (!eventId.HasValue || i.EventTeams.Any(j => j.EventId == eventId)))
                .ToListAsync();
            return result;
        }

        public async Task<List<Team>> GetAllThenTeamPLayers(int? tenantId)
        {
            return await _repository.GetAll()
                .Include(i => i.EventTeams)
                .ThenInclude(j => j.Group)
                .Include(j => j.TeamPlayers)
                .Where(i => i.IsDeleted == false && (!tenantId.HasValue || i.TenantId == tenantId))
                .ToListAsync();
        }


        public void InsertOrUpdateRange(List<Team> models)
        {
            _repository.GetDbContext().UpdateRange(models);
        }

        public async Task<Team> Update(Team model)
        {
            return await _repository.UpdateAsync(model);

        }

        public async Task<Team> Insert(Team model)
        {
            return await _repository.InsertAsync(model);

        }
    }
}
