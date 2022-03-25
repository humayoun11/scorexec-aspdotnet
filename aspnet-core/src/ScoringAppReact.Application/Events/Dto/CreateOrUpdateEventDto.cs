using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Events.Dto
{
    public class CreateOrUpdateEventDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Organizor { get; set; }
        public string OrganizorContact { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
        public int EventType { get; set; }
        public int? TournamentType { get; set; }
        public int? NumberOfGroup { get; set; }
        public int? TenantId { get; set; }
    }
}
