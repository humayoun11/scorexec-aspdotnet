using System;
using System.Collections.Generic;
using System.Text;

namespace ScoringAppReact.LiveScore.Dto
{
    public class LiveScoreDto
    {
        public long MatchId { get; set; }
        public int CurrentInning { get; set; }
        public long PlayingTeamId { get; set; }
        public long StrikerId { get; set; }
        public BatsmanDto Striker { get; set; }
        public BatsmanDto NonStriker { get; set; }
        public BowlerDto Bowler { get; set; }
        public ExtrasDto Extras { get; set; }
        public LiveTeamDto Team1 { get; set; }
        public LiveTeamDto Team2 { get; set; }
        public WicketDto HowOut { get; set; }

    }
}
