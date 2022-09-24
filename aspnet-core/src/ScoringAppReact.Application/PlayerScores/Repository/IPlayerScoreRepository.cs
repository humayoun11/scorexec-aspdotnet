using Abp.Domain.Services;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.PlayerScores.Repository
{
    public interface IPlayerScoreRepository
    {
        Task<List<PlayerScore>> GetAll(long? teamId, long matchId, int? tenantId);

        Task<PlayerScore> Get(long id);
    }
}
