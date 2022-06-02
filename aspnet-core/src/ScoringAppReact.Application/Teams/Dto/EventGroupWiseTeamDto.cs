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
    public class EventGroupWiseTeamDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public int? Group { get; set; }
    }
}