using Abp.Domain.Services;
using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.PlayerScores.Repository
{
    public interface IPlayerScoreRepository : IDomainService
    {
        Task<List<PlayerScore>> GetAll(long? teamId, long? matchId, long? player1Id, long? player2Id, int? tenantId, bool playerInclude = false, bool bowlerInclude = false);

        Task<PlayerScore> Get(long id);

        Task<List<PlayerScore>> GetAllPlayers(long? matchId, long? eventId, int? tenantId);

        void InsertOrUpdateRange(List<PlayerScore> models);

        Task<PlayerScore> Insert(PlayerScore model);

        Task<PlayerScore> Update(PlayerScore model);
    }
}
