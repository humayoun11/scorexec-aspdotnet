using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ScoringAppReact.Authorization;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using Abp;
using Abp.Runtime.Session;
using ScoringAppReact.MatchSummary.Dto;

namespace ScoringAppReact.MatchSummary
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class MatchSummaryAppService : AbpServiceBase, IMatchSummaryAppService
    {
        private readonly IRepository<PlayerScore, long> _playerScoreRepository;
        private readonly IRepository<TeamScore, long> _teamScoreRepository;
        private readonly IAbpSession _abpSession;

        public MatchSummaryAppService(IRepository<PlayerScore, long> repository,
            IRepository<TeamScore, long> scoreRepository,
            IAbpSession abpSession)
        {
            _teamScoreRepository = scoreRepository;
            _playerScoreRepository = repository;
            _abpSession = abpSession;
        }

        public async Task<MatchDetails> GetTeamScorecard(long team1Id, long team2Id, long matchId)
        {
            var playerScore = await _playerScoreRepository.GetAll().Where(i => i.MatchId == matchId && i.IsDeleted == false).ToListAsync();
            var teamScores = await _teamScoreRepository.GetAll().Where(i => i.MatchId == matchId && i.IsDeleted == false).ToListAsync();

            var team1Players = playerScore.Where(i => i.TeamId == team1Id).OrderBy(i => i.Position);
            var team2Players = playerScore.Where(i => i.TeamId == team2Id).OrderBy(i => i.Position);

            var team1Bowler = team1Players.Where(i => i.Overs.HasValue);
            var team2Bowler = team2Players.Where(i => i.Overs.HasValue);
            var matchDetail = new MatchDetails
            {
                FirstInningBatsman = team1Players.Select(i => new MatchBatsman()
                {
                    PlayerId = i.PlayerId,
                    PlayerName = i.Player.Name,
                    Runs = i.Bat_Runs,
                    Balls = i.Bat_Balls,
                    Four = i.Four,
                    Six = i.Six,
                    HowOut = i.HowOutId

                }).ToList(),
                SecondInningBatsman = team2Players.Select(i => new MatchBatsman()
                {
                    PlayerId = i.PlayerId,
                    PlayerName = i.Player.Name,
                    Runs = i.Bat_Runs,
                    Balls = i.Bat_Balls,
                    Four = i.Four,
                    Six = i.Six,
                    HowOut = i.HowOutId

                }).ToList(),
                FirstInningBowler = team2Bowler.Select(i => new MatchBowler()
                {
                    PlayerId = i.PlayerId,
                    PlayerName = i.Player.Name,
                    Overs = i.Overs,
                    Runs = i.Ball_Runs,
                    Wickets = i.Wickets

                }).ToList(),
                SecondInningBowler = team1Bowler.Select(i => new MatchBowler()
                {
                    PlayerId = i.PlayerId,
                    PlayerName = i.Player.Name,
                    Overs = i.Overs,
                    Runs = i.Ball_Runs,
                    Wickets = i.Wickets

                }).ToList(),
                Team1Score = teamScores.Where(i=> i.TeamId == team1Id).Select(i=> new TeamScoreDto()
                {
                    Score = i.TotalScore,
                    Name = i.Team.Name,
                    Overs= i.Overs

                }).FirstOrDefault(),
                Team2Score = teamScores.Where(i => i.TeamId == team2Id).Select(i => new TeamScoreDto()
                {
                    Score = i.TotalScore,
                    Name = i.Team.Name,
                    Overs = i.Overs

                }).FirstOrDefault(),
            };
            return matchDetail;
        }
    }
}

