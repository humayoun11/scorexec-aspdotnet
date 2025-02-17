using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Models;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Grounds.Dto
{
    public class GroundDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string ProfileUrl { get; set; }
        public List<GalleryDto> Pictures { get; set; }
    }
}