using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Players.Dto
{
    public class PlayerStatsInput
    {
        public long PlayerId { get; set; }
        public int? MatchType { get; set; }
        public int? Season { get; set; }
        public long? TeamId { get; set; }
    }
}
