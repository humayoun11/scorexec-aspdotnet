using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Teams.Dto
{
    public class TeamEditDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Place { get; set; }
        public string Zone { get; set; }
        public string Contact { get; set; }
        public bool IsRegistered { get; set; }
        public string City { get; set; }
        public string FileName { get; set; }
    }
}