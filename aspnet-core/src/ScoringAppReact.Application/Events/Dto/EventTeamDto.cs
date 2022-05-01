using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Events.Dto
{
    public class EventTeamDto
    {
        public long EventId { get; set; }
        public List<long> TeamIds { get; set; }

        public bool IsCreateMatch { get; set; }
    }
}
