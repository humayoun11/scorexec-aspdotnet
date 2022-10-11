using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Extensions;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using ScoringAppReact.TeamScores.Dto;
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
                     (!matchId.HasValue || i.MatchId == matchId) &&
                     (!tenantId.HasValue || i.TenantId == tenantId) &&
                     (!id.HasValue || i.Id == id) && i.IsDeleted == false)
                 .FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<TeamScore>> GetAll(long? matchId, int? tenantId, bool teamInclude)
        {
            return await _repository.GetAll()
                   .IncludeIf(teamInclude, i => i.Team)
                   .Where(i => (!matchId.HasValue || i.MatchId == matchId)
                   && (!tenantId.HasValue || i.TenantId == tenantId)
                   && i.IsDeleted == false).ToListAsync();
        }


        public async Task<TeamScore> Create(CreateOrUpdateTeamScoreDto model, int? tenantId)
        {
            var result = await _repository.InsertAsync(new TeamScore()
            {
                TotalScore = model.TotalScore,
                Overs = model.Overs,
                Wickets = model.Wickets,
                Wideballs = model.Wideballs,
                NoBalls = model.NoBalls,
                Byes = model.Byes,
                LegByes = model.LegByes,
                TeamId = model.TeamId,
                MatchId = model.MatchId,
                TenantId = tenantId

            });
            return result;
        }

        public void InsertOrUpdateRange(List<TeamScore> models)
        {
            _repository.GetDbContext().UpdateRange(models);
        }

        public async Task<TeamScore> Update(TeamScore model)
        {
            return await _repository.UpdateAsync(model);

        }

        public async Task<TeamScore> Insert(TeamScore model)
        {
            return await _repository.InsertAsync(model);

        }
    }
}
