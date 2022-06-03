using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Events.Dto
{
    public class CreateGroupWiseTeams
    {
        public long EventId { get; set; }
        public List<List<long>> SelectedTeams { get; set; }
    }
}
