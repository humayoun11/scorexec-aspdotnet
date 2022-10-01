using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.LiveScore.Dto
{
    public class InputWicketDto
    {
        public int HowOutId { get; set; }
        public long BatsmanId { get; set; }
        public long MatchId { get; set; }
        public long Team1Id { get; set; }
        public long Team2Id { get; set; }
        public long? BowlerId { get; set; }
        public long? FielderId { get; set; }
        public int? Runs { get; set; }
    }
}
