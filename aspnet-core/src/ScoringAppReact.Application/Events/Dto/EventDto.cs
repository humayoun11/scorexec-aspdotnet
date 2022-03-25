using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Models;

namespace ScoringAppReact.Events.Dto
{
    public class EventDto : EntityDto<long>
    {
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