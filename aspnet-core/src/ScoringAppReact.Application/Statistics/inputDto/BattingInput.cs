using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.Statistics.Dto
{
    public class BattingInput
    {
        public long? TeamId { get; set; }
        public int? Season { get; set; }
        public double? Overs { get; set; }
        public int? Position { get; set; }
        public int? MatchType { get; set; }
        public long? EventId { get; set; }
        public int? PlayerRoleId { get; set; }
    }
}
