using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Players.Dto
{
    public class PlayerListDto
    {
        public long Id { get; set; }
        public int? HowOutId { get; set; }
        public string Name { get; set; }
        public string ProfileUrl { get; set; }
        public long TeamId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
