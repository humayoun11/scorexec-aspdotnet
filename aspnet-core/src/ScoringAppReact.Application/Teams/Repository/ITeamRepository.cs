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

        Task<List<Team>> GetAll(int? tenantId);
    }
}
