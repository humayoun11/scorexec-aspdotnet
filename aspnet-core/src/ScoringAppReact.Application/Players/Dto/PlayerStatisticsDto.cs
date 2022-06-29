using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace ScoringAppReact.Players.Dto
{
    public class PlayerStatisticsDto
    {
        public int playerId { get; set; }
        public string PlayerName { get; set; }
        public string ProfileUrl { get; set; }
        public int? PlayerRole { get; set; }
        public int? BattingStyle { get; set; }
        public int? BowlingStyle { get; set; }
        public double DOB { get; set; }
        public int TotalMatch { get; set; }
        public int TotalInnings { get; set; }
        public int TotalBatRuns { get; set; }
        public int TotalBatBalls { get; set; }
        public int TotalFours { get; set; }
        public int TotalSixes { get; set; }
        public int TotalNotOut { get; set; }
        public int GetBowled { get; set; }
        public int GetHitWicket { get; set; }
        public int GetLBW { get; set; }
        public int GetCatch { get; set; }
        public int GetStump { get; set; }
        public int GetRunOut { get; set; }
        public int NumberOf50s { get; set; }
        public int NumberOf100s { get; set; }
        public string StrikeRate { get; set; }
        public string BattingAverage { get; set; }
        public int TotalOvers { get; set; }
        public int TotalBallRuns { get; set; }
        public int TotalWickets { get; set; }
        public int TotalMaidens { get; set; }
        public string BowlingAvg { get; set; }
        public string Economy { get; set; }
        public string FileName { get; set; }
        public int FiveWickets { get; set; }
        public int DoBowled { get; set; }
        public int DoHitWicket { get; set; }
        public int DoLBW { get; set; }
        public int DoCatch { get; set; }
        public int DoStump { get; set; }
        public int OnFieldCatch { get; set; }
        public int OnFieldStump { get; set; }
        public int OnFieldRunOut { get; set; }
        public int HightScore { get; set; }
        public int MostWickets { get; set; }
        public int BestBowlingFigureRuns { get; set; }
    }
}
