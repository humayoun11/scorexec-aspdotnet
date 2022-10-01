using ScoringAppReact.Models;
using ScoringAppReact.TeamScores.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.TeamScores.Repository
{
    public interface ITeamScoreRepository
    {
        Task<TeamScore> Get(long? id = null, long? teamId = null, long? matchId = null, int? tenantId = null);

        Task<List<TeamScore>> GetAll(long? matchId, int? tenantId, bool teamInclude);

        Task<TeamScore> Create(CreateOrUpdateTeamScoreDto model, int? tenantId);


        void InsertOrUpdateRange(List<TeamScore> models);

        Task<TeamScore> Insert(TeamScore model);

        Task<TeamScore> Update(TeamScore model);
    }
}
