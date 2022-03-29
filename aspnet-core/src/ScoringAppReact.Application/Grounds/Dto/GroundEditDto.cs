using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using ScoringAppReact.Authorization.Roles;

namespace ScoringAppReact.Grounds.Dto
{
    public class GroundEditDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Location { get; set; }
    }
}