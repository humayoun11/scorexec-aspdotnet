using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScoringAppReact.Models
{
    public class Event : FullAuditedEntity<long>, IMayHaveTenant
    {
        public Event()
        {
            Matches = new List<Match>();
        }

        [Required]
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Organizor { get; set; }
        public string OrganizorContact { get; set; }
        public long? StartDate { get; set; }
        public long? EndDate { get; set; }
        public int EventType { get; set; }
        public int? TournamentType { get; set; }
        public int? NumberOfGroup { get; set; }
        //public int? NumberOfTeam { get; set; }
        public List<Match> Matches { get; set; }
        public List<EventTeam> EventTeams { get; set; }
        public int? TenantId { get; set; }
    }
}