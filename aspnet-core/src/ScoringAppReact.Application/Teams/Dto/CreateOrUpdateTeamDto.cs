using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Teams.Dto
{
    public class CreateOrUpdateTeamDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public string Zone { get; set; }
        public int Type { get; set; }
        public string Contact { get; set; }
        public bool IsRegistered { get; set; }
        public string City { get; set; }
        public string FileName { get; set; }
        public int? TenantId { get; set; }
    }
}
