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
    public class GroupWiseTeamsDto
    {
        public List<EventGroupWiseTeamDto> Teams { get; set; }
    }
}