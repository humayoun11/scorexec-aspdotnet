using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Players.Repository
{
    public interface IPlayerRepository
    {

        Task<Player> Get(long id);

        Task<List<Player>> GetAll(int? tenantId, long? teamId);

        Task<List<Player>> GetAllByTeamIds(List<long> teamIds, int? tenantId);
    }
}
