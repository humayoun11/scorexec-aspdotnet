using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.Teams.InputDto
{
    public class StatsInput
    {
        public long? EventId { get; set; }
        public int? MatchTypeId { get; set; }
        public long TeamId { get; set; }
        public int? Season { get; set; }
    }
}
