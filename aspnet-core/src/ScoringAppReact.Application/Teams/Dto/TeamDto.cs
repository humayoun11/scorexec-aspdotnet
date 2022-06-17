using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Models;

namespace ScoringAppReact.Teams.Dto
{
    public class TeamDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Place { get; set; }
        public string Zone { get; set; }
        public string Contact { get; set; }
        public bool IsRegistered { get; set; }
        public string City { get; set; }
        public string ProfileUrl { get; set; }
        public int Type { get; set; }
        public List<Player> Players { get; set; }
        public List<GalleryDto> Pictures { get; set; }
    }
}