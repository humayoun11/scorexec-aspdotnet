using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.LiveScore.Dto
{
    public class TeamDto
    {
        public long TeamId { get; set; }
        public string Name { get; set; }
        public int? Runs { get; set; }
        public float? Overs { get; set; }
        public int? Wickets { get; set; }

    }
}
