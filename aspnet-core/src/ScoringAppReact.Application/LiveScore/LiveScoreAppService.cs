using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ScoringAppReact.Authorization;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using Abp;
using Abp.Runtime.Session;
using ScoringAppReact.LiveScore.Dto;
using System;
using Abp.UI;
using System.Collections.Generic;
using Abp.EntityFrameworkCore.Repositories;
using ScoringAppReact.PlayerScores.Repository;
using ScoringAppReact.TeamScores.Repository;

namespace ScoringAppReact.LiveScore
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class PlayerScoreAppService : AbpServiceBase, ILiveScoreAppService
    {
        private readonly IRepository<PlayerScore, long> _repository;
        private readonly IRepository<TeamScore, long> _teamRepository;
        private readonly IRepository<Match, long> _matchRepository;
        private readonly IAbpSession _abpSession;
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly ITeamScoreRepository _teamScoreRepository;

        public PlayerScoreAppService(
            IRepository<PlayerScore, long> repository,
            IRepository<TeamScore, long> teamRepository,
            IRepository<Match, long> matchRepository,
            IAbpSession abpSession,
            IPlayerScoreRepository playerScoreRepository,
            ITeamScoreRepository teamScoreRepository
            )
        {
            _repository = repository;
            _matchRepository = matchRepository;
            _abpSession = abpSession;
            _playerScoreRepository = playerScoreRepository;
            _teamScoreRepository = teamScoreRepository;
            _teamRepository = teamRepository;
        }

        public async Task<LiveScoreDto> Get(long matchId)
        {
            try
            {
                var match = await _matchRepository.GetAll()
               .Include(i => i.MatchDetail)
               .Include(i => i.TeamScores)
               .Where(i => i.Id == matchId && i.IsDeleted == false)
               .FirstOrDefaultAsync();

                var playerScores = await _playerScoreRepository.GetAll(null, matchId, _abpSession.TenantId);

                var battingTeamId = match.MatchDetail.Inning.Value == MatchStatus.FirstInning ?
                    match.HomeTeamId : match.OppponentTeamId;
                var bowlingTeamId = match.MatchDetail.Inning.Value == MatchStatus.SecondInning ?
                    match.HomeTeamId : match.OppponentTeamId;

                var team1Players = playerScores.Where(i => i.TeamId == battingTeamId && i.HowOutId == HowOut.Not_Out);


                var striker = GetBatsman(team1Players, true);

                var nonStriker = GetBatsman(team1Players, false);


                var bowler = GetBowler(playerScores, bowlingTeamId);

                var team1 = GetTeamScore(match.TeamScores, battingTeamId);
                var team2 = GetTeamScore(match.TeamScores, bowlingTeamId);


                var liveScore = new LiveScoreDto
                {
                    CurrentInning = match.MatchDetail.Inning.Value,
                    Bowler = bowler,
                    Striker = striker,
                    NonStriker = nonStriker,
                    Team1 = team1,
                    Team2 = team2,
                    MatchId = matchId
                };
                return liveScore;

            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong", e);

            }

        }

        public async Task<LiveScoreDto> UpdateLiveScore(LiveScoreDto model)
        {
            try
            {
                var players = await _playerScoreRepository.GetAll(null, model.MatchId, _abpSession.TenantId);

                var isStriker = UpdateStriker(players, model);

                var isBowler = UpdateBowler(players, model);

                var teamScore = await _teamScoreRepository.Get(matchId: model.MatchId, teamId: model.PlayingTeamId);

                var isTeamScore = UpdateTeamScore(teamScore = new TeamScore(), model);

                await UnitOfWorkManager.Current.SaveChangesAsync();

                if (!isStriker || !isBowler || !isTeamScore)
                {
                    throw new UserFriendlyException("Something went wrong while updating score please try again");
                }

                return await Get(model.MatchId);

            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while updateing, score please try again", e);
            }

        }

        public async Task<List<PlayerScore>> GetPlayers(long matchId, long teamId)
        {
            try
            {
                var bowlers = await _playerScoreRepository.GetAll(teamId, matchId, _abpSession.TenantId);
                return bowlers;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while getting, GetPlayers", e);
            }
        }

        //private methods
        private BatsmanDto GetBatsman(IEnumerable<PlayerScore> team1Players, bool isStriker)
        {
            return team1Players.Where(i => i.IsStriker == isStriker).Select(i => new BatsmanDto()
            {
                Id = i.Id,
                Balls = i.Ball_Runs,
                Runs = i.Bat_Runs,
                Fours = i.Four,
                Sixes = i.Six,
                Name = i.Player.Name,
                Dots = i.Bat_Dots
            }).FirstOrDefault();

        }

        private TeamDto GetTeamScore(List<TeamScore> model, long teamId)
        {
            return model
                    .Where(i => i.TeamId == teamId)
                    .Select(i => new TeamDto()
                    {
                        TeamId = i.TeamId,
                        Name = i.Team.Name,
                        Runs = i.TotalScore,
                        Overs = i.Overs,
                        Wickets = i.Wickets,

                    }).FirstOrDefault();
        }

        private BowlerDto GetBowler(List<PlayerScore> model, long teamId)
        {
            return model.Where(i => i.TeamId == teamId && i.IsBowling)
                .Select(i => new BowlerDto
                {
                    Id = i.Id,
                    Name = i.Player.Name,
                    Overs = i.Overs,
                    Wickets = i.Wickets,
                    Maidens = i.Maiden,
                    Dots = i.Ball_Dots,
                    Balls = int.Parse(i.ToString().Split('.')[1]),
                    TotalBalls = (int)i.Overs * 6 + int.Parse(i.Overs.ToString().Split('.')[1])
                }).FirstOrDefault();
        }

        private bool UpdateStriker(List<PlayerScore> players, LiveScoreDto model)
        {
            try
            {
                var striker = new PlayerScore();

                if (players.Any())
                {
                    striker = players
                     .Where(i => i.PlayerId == model.Striker.Id
                      && i.TeamId == model.PlayingTeamId)
                     .FirstOrDefault();
                }

                striker.PlayerId = model.Striker.Id;
                striker.Bat_Runs = model.Striker.Runs;
                striker.Six = model.Striker.Sixes;
                striker.Four = model.Striker.Fours;
                striker.Bat_Balls = model.Striker.Balls;
                striker.Bat_Dots = model.Striker.Dots;
                striker.TeamId = model.PlayingTeamId;
                striker.HowOutId = model.HowOut?.HowOutId;
                striker.BowlerId = model.HowOut?.BowlerId;
                striker.Fielder = model.HowOut?.FielderId.ToString();
                _repository.InsertAsync(striker);
                return true;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while updating, striking", e);
            }

        }

        private bool UpdateBowler(List<PlayerScore> players, LiveScoreDto model)
        {
            try
            {
                var bowler = new PlayerScore();
                if (players.Any())
                {
                    bowler = players
                        .Where(i => i.PlayerId == model.Bowler.Id)
                        .FirstOrDefault();
                }

                bowler.PlayerId = model.Bowler.Id;
                bowler.Ball_Runs = model.Bowler.Runs;
                bowler.Overs = model.Bowler.Overs;
                bowler.Maiden = model.Bowler.Maidens;
                bowler.Wickets = model.Bowler.Wickets;
                bowler.Ball_Dots = model.Bowler.Dots;
                bowler.TeamId = model.Team2.TeamId;
                _repository.InsertAsync(bowler);
                return true;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while updating, bowler", e);
            }

        }

        private bool UpdateTeamScore(TeamScore teamScore, LiveScoreDto model)
        {
            try
            {
                var score = new TeamScore();
                if (teamScore != null)
                {
                    score = teamScore;
                }
                score.TotalScore = model.Team1.Runs;
                score.Wickets = model.Team1.Wickets;
                score.Overs = model.Team1.Overs;
                score.Wideballs = model.Extras.Wides;
                score.NoBalls = model.Extras.NoBalls;
                score.Byes = model.Extras.Byes;
                score.LegByes = model.Extras.LegByes;
                score.MatchId = model.MatchId;
                score.TeamId = model.PlayingTeamId;

                _teamRepository.InsertAsync(score);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

    }
}

