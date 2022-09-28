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

        Task<TeamScore> Create(CreateOrUpdateTeamScoreDto model, int? tenantId);
    }
}
