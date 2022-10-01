using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using ScoringAppReact.Authorization;
using ScoringAppReact.Models;
using Abp;
using Abp.Runtime.Session;
using ScoringAppReact.LiveScore.Dto;
using System;
using Abp.UI;
using System.Collections.Generic;
using ScoringAppReact.PlayerScores.Repository;
using ScoringAppReact.TeamScores.Repository;
using Abp.Domain.Uow;
using ScoringAppReact.Teams.Repository;
using ScoringAppReact.Matches.Repository;

namespace ScoringAppReact.LiveScore
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class LiveScoreAppService : AbpServiceBase, ILiveScoreAppService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IAbpSession _abpSession;
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly ITeamScoreRepository _teamScoreRepository;
        private readonly ITeamRepository _teamRepository;

        public LiveScoreAppService(
            IMatchRepository matchRepository,
            IAbpSession abpSession,
            IPlayerScoreRepository playerScoreRepository,
            ITeamScoreRepository teamScoreRepository,
            ITeamRepository teamRepository
            )
        {
            _matchRepository = matchRepository;
            _abpSession = abpSession;
            _playerScoreRepository = playerScoreRepository;
            _teamScoreRepository = teamScoreRepository;
            _teamRepository = teamRepository;
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<LiveScoreDto> Get(long matchId, bool newOver = false)
        {
            try
            {
                var match = await _matchRepository.GetMatchThen(matchId);

                if (match == null)
                {
                    throw new UserFriendlyException("Match and details not found");
                }

                var playerScores = await _playerScoreRepository.GetAll(null, matchId, null, null, _abpSession.TenantId, playerInclude: true);

                if (playerScores == null)
                {
                    throw new UserFriendlyException("playerScores not found");
                }

                var battingTeamId = match.MatchDetail.Inning.Value == MatchStatus.FirstInning ?
                    match.HomeTeamId : match.OppponentTeamId;

                if (battingTeamId == 0)
                {
                    throw new UserFriendlyException("battingTeamId not found");
                }
                var bowlingTeamId = match.MatchDetail.Inning.Value == MatchStatus.SecondInning ?
                    match.HomeTeamId : match.OppponentTeamId;

                if (bowlingTeamId == 0)
                {
                    throw new UserFriendlyException("bowlingTeamId not found");
                }

                var team1Players = playerScores.Where(i => i.TeamId == battingTeamId && i.HowOutId == HowOut.Not_Out);


                if (team1Players == null)
                {
                    throw new UserFriendlyException("team1Players not found");
                }

                var striker = GetBatsman(team1Players, true);

                if (striker == null)
                {
                    throw new UserFriendlyException("striker not found");
                }

                var nonStriker = GetBatsman(team1Players, false);

                if (nonStriker == null)
                {
                    throw new UserFriendlyException("nonStriker not found");
                }


                var bowler = GetBowler(playerScores, bowlingTeamId, newOver);

                if (bowler == null)
                {
                    throw new UserFriendlyException("bowler not found");
                }


                var team1 = await GetTeamScore(match.TeamScores, battingTeamId);

                if (team1 == null)
                {
                    throw new UserFriendlyException("team1 not found");
                }

                var team2 = await GetTeamScore(match.TeamScores, bowlingTeamId);

                if (team2 == null)
                {
                    throw new UserFriendlyException("team2 not found");
                }


                Dictionary<long, BatsmanDto> Batsmans = new Dictionary<long, BatsmanDto>
                {
                    { striker.Id, striker },
                    { nonStriker.Id, nonStriker }
                };

                var extras = new ExtrasDto
                {
                    Wides = 1,
                    Byes = 1,
                    NoBalls = 1,
                    LegByes = 1
                };
                var liveScore = new LiveScoreDto
                {
                    CurrentInning = match.MatchDetail.Inning.Value,
                    Bowler = bowler,
                    Batsmans = Batsmans,
                    Team1 = team1,
                    Team2 = team2,
                    MatchId = matchId,
                    StrikerId = striker.Id,
                    PlayingTeamId = battingTeamId,
                    Extras = extras,
                    BowlingTeamId = bowlingTeamId
                };
                return liveScore;

            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong", e);

            }

        }

        public async Task<List<PlayerScore>> GetPlayers(long matchId, long teamId)
        {
            try
            {
                var bowlers = await _playerScoreRepository.GetAll(teamId, matchId, null, null, _abpSession.TenantId);
                return bowlers;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while getting, GetPlayers", e);
            }
        }

        public async Task<LiveScoreDto> Submit(InputLiveScoreDto model)
        {
            var players = await _playerScoreRepository.GetAll(null, model.MatchId, null, null, _abpSession.TenantId);

            switch (model.Extras)
            {
                case Extras.NO_EXTRA:
                    await UpdateTeamScore(model, Ball.LEGAL);
                    await UpdateBowler(players, model.Team2Id, model.BowlerId, model.Runs, Ball.LEGAL);
                    await UpdateStriker(players, model.Team1Id, model.BatsmanId, model.Runs, Ball.LEGAL, IsChangeStrike(model.Runs));
                    break;
                case Extras.WIDE:
                    await UpdateTeamScore(model, Ball.ILL_LEGAL);
                    await UpdateBowler(players, model.Team2Id, model.BowlerId, model.Runs + 1, Ball.ILL_LEGAL);
                    break;
                case Extras.NO_BALLS:
                    await UpdateTeamScore(model, Ball.ILL_LEGAL);
                    await UpdateStriker(players, model.Team1Id, model.BatsmanId, model.Runs, Ball.LEGAL, IsChangeStrike(model.Runs));
                    await UpdateBowler(players, model.Team2Id, model.BowlerId, model.Runs + 1, Ball.ILL_LEGAL);
                    break;
                case Extras.BYES:
                    await UpdateTeamScore(model, 1);
                    await UpdateBowler(players, model.Team2Id, model.BowlerId, Run.DOT, Ball.LEGAL);
                    await UpdateStriker(players, model.Team1Id, model.BatsmanId, Run.DOT, Ball.LEGAL, IsChangeStrike(model.Runs));
                    break;
                case Extras.LEG_BYES:
                    await UpdateTeamScore(model, Ball.LEGAL);
                    await UpdateBowler(players, model.Team2Id, model.BowlerId, Run.DOT, Ball.LEGAL);
                    await UpdateStriker(players, model.Team1Id, model.BatsmanId, Run.DOT, Ball.LEGAL, IsChangeStrike(model.Runs));
                    break;
            }

            return await Get(model.MatchId);

        }

        //private methods
        private BatsmanDto GetBatsman(IEnumerable<PlayerScore> team1Players, bool isStriker)
        {
            return team1Players.Where(i => i.IsStriker == isStriker).Select(i => new BatsmanDto()
            {
                Id = i.PlayerId,
                Balls = i.Bat_Balls,
                Runs = i.Bat_Runs,
                Fours = i.Four,
                Sixes = i.Six,
                Name = i.Player.Name,
                Dots = i.Bat_Dots,
                Timeline = new int[10]
            }).FirstOrDefault();

        }

        private async Task<LiveTeamDto> GetTeamScore(List<TeamScore> model, long teamId)
        {
            try
            {
                var teamScore = new LiveTeamDto();
                var currentTeam = model.Where(i => i.TeamId == teamId).FirstOrDefault();

                if (currentTeam == null)
                {
                    var team = await _teamRepository.Get(teamId);

                    teamScore.TeamId = team.Id;
                    teamScore.Name = team.Name;
                    teamScore.Runs = 0;
                    teamScore.Overs = 0;
                    teamScore.Wickets = 0;
                }
                else
                {
                    teamScore = model
                  .Where(i => i.TeamId == teamId)
                  .Select(i => new LiveTeamDto()
                  {
                      TeamId = i.TeamId,
                      Name = i.Team.Name,
                      Runs = i.TotalScore,
                      Overs = i.Overs,
                      Wickets = i.Wickets,

                  }).FirstOrDefault();
                }

                return teamScore;


            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while GetTeamScore", e);
            }

        }

        private BowlerDto GetBowler(List<PlayerScore> model, long teamId, bool newOver)
        {
            try
            {
                var bowler = model.Where(i => i.TeamId == teamId && i.IsBowling == true).FirstOrDefault();
                if (bowler == null)
                {
                    throw new UserFriendlyException("Bowler not found");
                }

                int Balls = 0;
                var TotalBalls = 0;
                if (bowler.Overs != null && bowler.Overs != 0)
                {
                    var a = bowler.Overs.ToString().Split('.');
                    if (a.Count() > 1)
                    {
                        Balls = int.Parse(bowler.Overs.ToString().Split('.')[1]);

                        TotalBalls = (int)bowler.Overs * 6 + int.Parse(bowler.Overs.ToString().Split('.')[1]);
                    }
                    else
                    {
                        TotalBalls = (int)bowler.Overs * 6;
                    }

                }


                var mappedData = new BowlerDto
                {
                    Id = bowler.PlayerId,
                    Name = bowler.Player.Name,
                    Overs = bowler.Overs,
                    Runs = bowler.Ball_Runs,
                    Wickets = bowler.Wickets,
                    Maidens = bowler.Maiden,
                    Dots = bowler.Ball_Dots,
                    Balls = Balls,
                    TotalBalls = TotalBalls,
                    Timeline = new int[10],
                    NewOver = newOver
                };
                return mappedData;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while GetBowler", e);
            }



        }

        public async Task<LiveScoreDto> ChangeBowler(InputChangeBowler model)
        {
            var players = await _playerScoreRepository.GetAll(model.TeamId, model.MatchId, model.NewBowlerId, model.PrevBowlerId, _abpSession.TenantId);
            if (!players.Any())
            {
                throw new UserFriendlyException($"Players not found with associating ids {model.NewBowlerId} and {model.PrevBowlerId}");
            }
            foreach (var item in players)
            {
                if (item.PlayerId == model.PrevBowlerId)
                    item.IsBowling = false;
                if (item.PlayerId == model.NewBowlerId)
                    item.IsBowling = true;
            }

            _playerScoreRepository.InsertOrUpdateRange(players);

            return await Get(model.MatchId, true);
        }

        private async Task<bool> UpdateStriker(List<PlayerScore> players, long teamId, long batsmanId, int runs, int balls, bool IsChangeStrike)
        {
            try
            {
                var strikers = new List<PlayerScore>();
                var changeStrike = false;
                if (IsChangeStrike)
                {
                    changeStrike = true;
                    var nonStriker = players
                     .Where(i => i.PlayerId != batsmanId
                      && i.TeamId == teamId)
                     .FirstOrDefault();

                    nonStriker.IsStriker = changeStrike;
                    strikers.Add(nonStriker);
                }


                var striker = players
                 .Where(i => i.PlayerId == batsmanId
                  && i.TeamId == teamId)
                 .FirstOrDefault();



                striker.Bat_Runs += runs;
                striker.Six += runs == 6 ? 1 : 0;
                striker.Four += runs == 4 ? 1 : 0;
                striker.Bat_Balls += balls;
                striker.IsStriker = !changeStrike;

                strikers.Add(striker);

                _playerScoreRepository.InsertOrUpdateRange(strikers);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong while updating, striking", e);
            }

        }

        private async Task<bool> UpdateBowler(List<PlayerScore> players, long teamId, long bowlerId, int runs, int ball)
        {
            var bowler = new PlayerScore();

            bowler = players
             .Where(i => i.PlayerId == bowlerId
              && i.TeamId == teamId)
             .FirstOrDefault();

            bowler.Ball_Runs += runs;
            bowler.Overs = float.Parse($"{CalculateOvers(bowler.Overs, ball).Item1}.{CalculateOvers(bowler.Overs, ball).Item2}");
            //bowler.Maiden += 0;  todo

            await _playerScoreRepository.Update(bowler);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return true;

        }

        private async Task<bool> UpdateTeamScore(InputLiveScoreDto model, int ball)
        {
            try
            {
                var teamScore = await _teamScoreRepository.Get(matchId: model.MatchId, teamId: model.Team1Id);


                teamScore.TotalScore += model.Runs;
                teamScore.Overs = float.Parse($"{CalculateOvers(teamScore.Overs, ball).Item1}.{CalculateOvers(teamScore.Overs, ball).Item2}");

                await _teamScoreRepository.Update(teamScore);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Tuple<int, int> CalculateOvers(float? over, int ball)
        {
            var balls = 0;
            var overs = 0;

            if (over != null && over != 0)
            {
                var a = over.ToString().Split('.');
                if (a.Count() > 1)
                {
                    balls = int.Parse(over.ToString().Split('.')[1]);

                }

                overs = (int)over;
            }


            balls += ball;

            if (balls >= 6)
            {
                overs++;
                balls = 0;
            }

            return Tuple.Create(overs, balls);

        }

        private bool IsChangeStrike(int runs)
        {
            return runs % 2 != 0 ? true : false;
        }

        private async Task<LiveScoreDto> ChangeBatsman(InputWicketDto model)
        {
            try
            {
                var playerScore = new List<PlayerScore>();
                var players = await _playerScoreRepository.GetAll(null, model.MatchId, null, null, _abpSession.TenantId);
               
                var batsman = players.Where(i => i.PlayerId == model.BatsmanId && i.TeamId == model.Team1Id).SingleOrDefault();
                var bowler = players.Where(i => i.PlayerId == model.BowlerId && i.TeamId == model.Team2Id).SingleOrDefault();
                //teamscore

                switch (model.HowOutId)
                {
                    case HowOut.Stump:
                        batsman.BowlerId = model.BowlerId;
                        batsman.Fielder = model.FielderId.ToString();
                        bowler.Wickets++;
                        break;
                    case HowOut.Bowled:
                        batsman.BowlerId = model.BowlerId;
                        bowler.Wickets++;
                        break;
                    case HowOut.Run_Out:
                        batsman.Fielder = model.FielderId.ToString();
                        batsman.Bat_Runs += model.Runs;
                        break;
                    case HowOut.Catch:
                        batsman.BowlerId = model.BowlerId;
                        batsman.Fielder = model.FielderId.ToString();
                        bowler.Wickets++;
                        break;
                    case HowOut.Hit_Wicket:
                        batsman.BowlerId = model.BowlerId;
                        bowler.Wickets++;
                        break;
                    case HowOut.LBW:
                        batsman.BowlerId = model.BowlerId;
                        bowler.Wickets++;
                        break;
                }

                batsman.HowOutId = model.HowOutId;
               
                playerScore.Add(batsman);
                playerScore.Add(bowler);

                _playerScoreRepository.InsertOrUpdateRange(playerScore);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return await Get(model.MatchId, false);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


    }
}

