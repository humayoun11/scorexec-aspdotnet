using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using Abp.AutoMapper;
using ScoringAppReact.Authorization.Roles;
using ScoringAppReact.Models;

namespace ScoringAppReact.Events.Dto
{
    public class PointsTableDto
    {
        public string Team { get; set; }
        public int? Played { get; set; }
        public int? Won { get; set; }
        public int? Lost { get; set; }
        public int? Tie { get; set; }
        public int? Points { get; set; }
        public float? RunRate { get; set; }
    }
}