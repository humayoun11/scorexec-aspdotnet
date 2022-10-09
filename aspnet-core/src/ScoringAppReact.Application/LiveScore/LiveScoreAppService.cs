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
using ScoringAppReact.Players.Repository;
using ScoringAppReact.Players.Dto;

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
        private readonly IPlayerRepository _playerRepository;

        public LiveScoreAppService(
            IMatchRepository matchRepository,
            IAbpSession abpSession,
            IPlayerScoreRepository playerScoreRepository,
            ITeamScoreRepository teamScoreRepository,
            ITeamRepository teamRepository,
            IPlayerRepository playerRepository
            )
        {
            _matchRepository = matchRepository;
            _abpSession = abpSession;
            _playerScoreRepository = playerScoreRepository;
            _teamScoreRepository = teamScoreRepository;
            _teamRepository = teamRepository;
            _playerRepository = playerRepository;
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<LiveScoreDto> Get(long matchId, bool newOver = false)
        {
            try
            {

                var players = new List<PlayerListDto>();
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



                var nonStriker = GetBatsman(team1Players, false);

                if (nonStriker == null || striker == null)
                {
                    players = await GetBatsmanOrBowlers(matchId, battingTeamId);
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


                Dictionary<long, BatsmanDto> Batsmans = new Dictionary<long, BatsmanDto>();

                if (striker != null)
                {
                    Batsmans.Add(striker.Id, striker);

                }
                if (nonStriker != null)
                {
                    Batsmans.Add(nonStriker.Id, nonStriker);
                }

                var extras = new ExtrasDto
                {
                    Wides = 1,
                    Byes = 1,
                    NoBalls = 1,
                    LegByes = 1
                };

                return new LiveScoreDto
                {
                    CurrentInning = match.MatchDetail.Inning.Value,
                    Bowler = bowler,
                    Batsmans = Batsmans,
                    Team1 = team1,
                    Team2 = team2,
                    MatchId = matchId,
                    StrikerId = striker?.Id,
                    PlayingTeamId = battingTeamId,
                    Extras = extras,
                    BowlingTeamId = bowlingTeamId,
                    Players = players
                };


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


        public async Task<LiveScoreDto> UpdateNewBatsman(InputNewBatsman model)
        {
            try
            {
                var players = await _playerScoreRepository.GetAll(model.TeamId, model.MatchId, null, null, _abpSession.TenantId);
                var preBatsman = players.Where(i => i.HowOutId == HowOut.Not_Out && i.IsPlayedInning == true).FirstOrDefault();
                var newBatsman = players.Where(i => i.HowOutId == HowOut.Not_Out && i.PlayerId == model.BatsmanId).FirstOrDefault();
                newBatsman.IsPlayedInning = true;
                if (preBatsman.IsStriker == false)
                    newBatsman.IsStriker = true;

                await _playerScoreRepository.Update(newBatsman);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return await Get(model.MatchId, true);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<List<PlayerListDto>> ChangeBatsman(InputWicketDto model)
        {
            try
            {
                if (model.BatsmanId == 0)
                {
                    throw new UserFriendlyException("BatsmanId cannot be zero or null");
                }

                if (model.StrikerId == 0)
                {
                    throw new UserFriendlyException("StrikerId cannot be zero or null");
                }
                var playerScore = new List<PlayerScore>();
                var players = await _playerScoreRepository.GetAll(null, model.MatchId, null, null, _abpSession.TenantId);

                var striker = players.Where(i => i.PlayerId == model.StrikerId && i.TeamId == model.Team1Id).SingleOrDefault();
                var bowler = players.Where(i => i.PlayerId == model.BowlerId && i.TeamId == model.Team2Id).SingleOrDefault();

                //teamscore
                var teamScore = await _teamScoreRepository.Get(model.Team1Id);



                var isTeamBall = true;
                var isBatsmanScore = true;
                var isBowlerScore = true;
                var isBowlerBall = true;
                var bowlerScore = model.Runs;
                var totalScore = model.Runs;

                switch (model.WideOrNoBall)
                {
                    case Extras.WIDE:
                        isBatsmanScore = false;
                        isTeamBall = false;
                        isBowlerBall = false;
                        bowlerScore = model.Runs + 1;
                        totalScore = model.Runs + 1;
                        break;
                    case Extras.NO_BALLS:
                        isBowlerScore = model.BuyOrlegBye == Extras.BYES || model.BuyOrlegBye == Extras.LEG_BYES;
                        isBatsmanScore = model.BuyOrlegBye == Extras.BYES || model.BuyOrlegBye == Extras.LEG_BYES;
                        isTeamBall = false;
                        isBowlerBall = false;
                        bowlerScore = model.BuyOrlegBye == Extras.BYES || model.BuyOrlegBye == Extras.LEG_BYES ? 1 : model.Runs + 1;
                        totalScore = model.Runs + 1;
                        break;
                }

                switch (model.HowOutId)
                {
                    case HowOut.Stump:
                        striker.BowlerId = model.BowlerId;
                        striker.Fielder = model.FielderId.ToString();
                        bowler.Wickets++;
                        bowler.Overs = isBowlerScore == true ? OverConcatinate(bowler.Overs, Ball.LEGAL) : bowler.Overs;
                        teamScore.Overs = isTeamBall == true ? OverConcatinate(teamScore.Overs, Ball.LEGAL) : teamScore.Overs;
                        break;
                    case HowOut.Bowled:
                        striker.BowlerId = model.BowlerId;
                        if (isBatsmanScore == true) striker.Bat_Balls++;
                        bowler.Wickets++;
                        bowler.Overs = isBowlerScore == true ? OverConcatinate(bowler.Overs, Ball.LEGAL) : bowler.Overs;
                        break;
                    case HowOut.Run_Out:
                        if (model.StrikerId == model.BatsmanId)
                        {
                            striker.Fielder = model.FielderId.ToString();
                            striker.HowOutId = model.HowOutId;
                            striker.IsStriker = false;

                        }
                        else
                        {
                            var whoOutPlayer = players.Where(i => i.PlayerId == model.BatsmanId && i.TeamId == model.Team1Id).SingleOrDefault();

                            whoOutPlayer.HowOutId = model.HowOutId;
                            whoOutPlayer.IsStriker = false;
                            whoOutPlayer.Fielder = model.FielderId.ToString();
                            playerScore.Add(whoOutPlayer);
                        }
                        striker.Bat_Runs += isBatsmanScore == true ? model.Runs : 0;
                        //todo
                        bowler.Overs = isBowlerScore == true ? OverConcatinate(bowler.Overs, isBowlerScore == true ? Ball.LEGAL : Ball.ILL_LEGAL) : bowler.Overs;
                        bowler.Ball_Runs += bowlerScore;
                        teamScore.Overs = OverConcatinate(teamScore.Overs, isTeamBall == true ? Ball.LEGAL : Ball.ILL_LEGAL);
                        teamScore.TotalScore += totalScore;
                        break;
                    case HowOut.Catch:
                        striker.BowlerId = model.BowlerId;
                        striker.Fielder = model.FielderId.ToString();
                        bowler.Wickets++;
                        bowler.Overs = isBowlerScore == true ? OverConcatinate(bowler.Overs, Ball.LEGAL) : bowler.Overs;
                        teamScore.Overs = OverConcatinate(teamScore.Overs, Ball.LEGAL);
                        break;
                    case HowOut.Hit_Wicket:
                        striker.BowlerId = model.BowlerId;
                        bowler.Wickets++;
                        bowler.Overs = isBowlerScore == true ? OverConcatinate(bowler.Overs, Ball.LEGAL) : bowler.Overs;
                        teamScore.Overs = OverConcatinate(teamScore.Overs, Ball.LEGAL);
                        break;
                    case HowOut.LBW:
                        striker.BowlerId = model.BowlerId;
                        bowler.Wickets++;
                        bowler.Overs = isBowlerScore == true ? OverConcatinate(bowler.Overs, Ball.LEGAL) : bowler.Overs;
                        teamScore.Overs = OverConcatinate(teamScore.Overs, Ball.LEGAL);
                        break;
                }

                if (model.StrikerId == model.BatsmanId)
                    striker.HowOutId = model.HowOutId;

                playerScore.Add(striker);
                playerScore.Add(bowler);

                await _teamScoreRepository.Update(teamScore);
                _playerScoreRepository.InsertOrUpdateRange(playerScore);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return await GetBatsmanOrBowlers(model.MatchId, model.Team1Id);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<List<PlayerListDto>> GetBatsmanOrBowlers(long matchId, long? teamId)
        {
            var result = await _playerScoreRepository.GetAll(teamId, matchId, null, null, _abpSession.TenantId, true, false);
            return result.Where(j => j.IsPlayedInning == false).Select(i => new PlayerListDto()
            {
                Id = i.PlayerId,
                Name = i.Player.Name,
                TeamId = i.TeamId,
                ProfileUrl = i.Player.ProfileUrl,
                HowOutId = i.HowOutId

            }).ToList();
        }

        //private methods
        private BatsmanDto GetBatsman(IEnumerable<PlayerScore> team1Players, bool isStriker)
        {
            return team1Players.Where(i => i.IsStriker == isStriker && i.HowOutId == HowOut.Not_Out).Select(i => new BatsmanDto()
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
            bowler.Overs = OverConcatinate(bowler.Overs, ball);
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
                teamScore.Overs = OverConcatinate(teamScore.Overs, ball);

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

        private float OverConcatinate(float? overs, int ball)
        {
            return float.Parse($"{CalculateOvers(overs, ball).Item1}.{CalculateOvers(overs, ball).Item2}");
        }


    }
}

