using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.LiveScore.Dto
{
    public class InputLiveScoreDto
    {
        public int Runs { get; set; }
        public long Team1Id { get; set; }
        public long Team2Id { get; set; }
        public long MatchId { get; set; }
        public long BatsmanId { get; set; }
        public long BowlerId { get; set; }
        public int? Extras { get; set; }

    }
}
