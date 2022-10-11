using ScoringAppReact.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ScoringAppReact.Teams.Repository
{
    public interface ITeamRepository
    {
        Task<Team> Get(long id);

        Task<List<Team>> GetAll(int? tenantId, long? eventId, bool eventTeamInclude = false);

        Task<List<Team>> GetAllThenTeamPLayers(int? tenantId);

        void InsertOrUpdateRange(List<Team> models);

        Task<Team> Insert(Team model);

        Task<Team> Update(Team model);
    }
}
