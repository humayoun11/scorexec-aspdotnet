using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.LiveScore.Dto
{
    public class InputChangeBowler
    {
        public long PrevBowlerId { get; set; }
        public long NewBowlerId { get; set; }
        public long MatchId { get; set; }
        public long TeamId { get; set; }
    }
}
