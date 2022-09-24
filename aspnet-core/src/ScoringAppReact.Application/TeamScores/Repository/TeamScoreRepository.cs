using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.TeamScores.Repository
{
    public class TeamScoreRepository : ITeamScoreRepository
    {
        private readonly IRepository<TeamScore, long> _repository;

        public TeamScoreRepository(
            IRepository<TeamScore, long> repository
           )
        {
            _repository = repository;
        }


        public async Task<TeamScore> Get(long? id = null, long? teamId = null, long? matchId = null, int? tenantId = null)
        {
            var result = await _repository.GetAll()
                 .Where(i =>
                     (!teamId.HasValue || i.TeamId == teamId) &&
                     (!matchId.HasValue || i.TeamId == matchId) &&
                     (!tenantId.HasValue || i.TenantId == tenantId) &&
                     (!id.HasValue || i.Id == id) && i.IsDeleted == false)
                 .FirstOrDefaultAsync();
            return result;
        }
    }
}
