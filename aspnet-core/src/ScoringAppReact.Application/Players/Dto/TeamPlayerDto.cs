using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Players.Dto
{
    public class TeamPlayerDto
    {
        public long TeamId { get; set; }
        public List<long> PlayerIds { get; set; }
    }
}
