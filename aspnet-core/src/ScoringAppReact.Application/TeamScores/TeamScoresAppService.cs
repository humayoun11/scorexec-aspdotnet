using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using ScoringAppReact.Authorization;
using ScoringAppReact.Models;
using Abp;
using Abp.Runtime.Session;
using Abp.UI;
using ScoringAppReact.TeamScores.Dto;
using ScoringAppReact.MatchSummary.Dto;
using ScoringAppReact.PlayerScores;
using ScoringAppReact.FallOfWickets;
using System;
using ScoringAppReact.Teams;
using ScoringAppReact.Teams.Dto;
using ScoringAppReact.TeamScores.Repository;
using ScoringAppReact.PlayerScores.Repository;
using ScoringAppReact.Matches.Repository;

namespace ScoringAppReact.TeamScores
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class TeamScoresAppService : AbpServiceBase, ITeamScoresAppService
    {
        private readonly IAbpSession _abpSession;
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly PlayerScoreAppService _playerscoreAppService;
        private readonly FallofWicketAppService _fallofWicketAppService;
        private readonly TeamAppService _teamAppService;
        private readonly ITeamScoreRepository _teamScoreRepository;

        public TeamScoresAppService(
            IMatchRepository matchRepository, IAbpSession abpSession,
            IPlayerScoreRepository playerScoreRepository,
            PlayerScoreAppService playerscoreAppService,
            FallofWicketAppService fallofWicketAppService,
            TeamAppService teamAppService,
            ITeamScoreRepository teamScoreRepository
            )
        {
            _abpSession = abpSession;
            _playerScoreRepository = playerScoreRepository;
            _matchRepository = matchRepository;
            _playerscoreAppService = playerscoreAppService;
            _fallofWicketAppService = fallofWicketAppService;
            _teamAppService = teamAppService;
            _teamScoreRepository = teamScoreRepository;
        }

        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateTeamScoreDto model)
        {
            ResponseMessageDto result;
            if (model.Id == 0 || model.Id == null)
            {
                result = await CreateTeamScoreAsync(model);
            }
            else
            {
                result = await UpdateTeamScoreAsync(model);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreateTeamScoreAsync(CreateOrUpdateTeamScoreDto model)
        {
            var alreadyAdded = await _teamScoreRepository.Get(matchId: model.MatchId, teamId: model.TeamId);
            if (alreadyAdded != null)
            {
                throw new UserFriendlyException("Already added with associated team and match");
            }


            var result = await _teamScoreRepository.Create(model, _abpSession.TenantId);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (result.Id != 0)
            {
                return new ResponseMessageDto()
                {
                    Id = result.Id,
                    SuccessMessage = AppConsts.SuccessfullyInserted,
                    Success = true,
                    Error = false,
                    result = result
                };
            }
            return new ResponseMessageDto()
            {
                Id = 0,
                ErrorMessage = AppConsts.InsertFailure,
                Success = false,
                Error = true,
            };
        }

        private async Task<ResponseMessageDto> UpdateTeamScoreAsync(CreateOrUpdateTeamScoreDto model)
        {
            var result = await _teamScoreRepository.Update(new TeamScore()
            {
                Id = model.Id.Value,
                TotalScore = model.TotalScore,
                Overs = model.Overs,
                Wickets = model.Wickets,
                Wideballs = model.Wideballs,
                NoBalls = model.NoBalls,
                Byes = model.Byes,
                LegByes = model.LegByes,
                TeamId = model.TeamId,
                MatchId = model.MatchId,
                TenantId = _abpSession.TenantId
            });

            if (result != null)
            {
                return new ResponseMessageDto()
                {
                    Id = result.Id,
                    SuccessMessage = AppConsts.SuccessfullyUpdated,
                    Success = true,
                    Error = false,
                    result = result
                };
            }
            return new ResponseMessageDto()
            {
                Id = 0,
                ErrorMessage = AppConsts.UpdateFailure,
                Success = false,
                Error = true,
            };
        }

        public async Task<TeamScoreDto> GetById(long id)
        {
            try
            {
                var result = await _teamScoreRepository.Get(id: id);
                return ObjectMapper.Map<TeamScoreDto>(result);
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        public async Task<ResponseMessageDto> DeleteAsync(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Id id required");
                //return;
            }
            var model = await _teamScoreRepository.Get(id);

            if (model == null)
            {
                throw new UserFriendlyException("No record found with associated Id");
                //return;
            }
            model.IsDeleted = true;
            var result = await _teamScoreRepository.Update(model);

            return new ResponseMessageDto()
            {
                Id = id,
                SuccessMessage = AppConsts.SuccessfullyDeleted,
                Success = true,
                Error = false,
            };
        }

        public async Task<TeamScoreDto> GetByTeamIdAndMatchId(long teamId, long matchId)
        {
            try
            {
                var result = await _teamScoreRepository.Get(teamId: teamId, matchId: matchId, tenantId: _abpSession.TenantId);
                return ObjectMapper.Map<TeamScoreDto>(result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public async Task<MatchDetails> GetTeamScorecard(long team1Id, long team2Id, long matchId)
        {
            try
            {
                var playerScore = await _playerScoreRepository.GetAll(null, matchId, null, null, _abpSession.TenantId, playerInclude: true, bowlerInclude: true);

                var teamScores = await _teamScoreRepository.GetAll(matchId, _abpSession.TenantId, teamInclude: true);

                var match = await _matchRepository.GetAll(matchId, groundInclude: true, eventInclude: true);


                var team1Players = playerScore.Where(i => i.TeamId == team1Id).OrderBy(i => i.Position).ToList();
                var team2Players = playerScore.Where(i => i.TeamId == team2Id).OrderBy(i => i.Position).ToList();

                var team1Bowler = team1Players.Where(i => i.Overs.HasValue);
                var team2Bowler = team2Players.Where(i => i.Overs.HasValue);

                var FirstInningBatsman = BatsmanMapper(team1Players);
                var SecondInningBatsman = BatsmanMapper(team2Players);

                var FirstInningBowler = BowlersMapper(team1Bowler);
                var SecondInningBowler = BowlersMapper(team2Bowler);

                var teams = new List<TeamDto>();
                var Team1Score = new TeamsScoreDto();
                var Team2Score = new TeamsScoreDto();
                if (!teamScores.Any())
                {
                    teams = await _teamAppService.GetAllTeamsByMatchId(matchId);
                    Team1Score = TeamScoreMapper(null, team1Id, teams);
                    Team2Score = TeamScoreMapper(null, team2Id, teams);
                }
                else
                {
                    Team1Score = TeamScoreMapper(teamScores, team1Id, null);
                    Team2Score = TeamScoreMapper(teamScores, team2Id, null);
                }


                var firstInningTop3Batsman = FirstInningBatsman.Where(i => i.Runs.HasValue).OrderByDescending(x => x.Runs).Take(3);
                var firstInningTop3Bowler = FirstInningBowler.Where(i => i.Wickets.HasValue).OrderByDescending(x => x.Wickets).Take(3);

                var secondInningTop3Batsman = SecondInningBatsman.Where(i => i.Runs.HasValue).OrderByDescending(x => x.Runs).Take(3);
                var secondInningTop3Bowler = SecondInningBowler.Where(i => i.Wickets.HasValue).OrderByDescending(x => x.Wickets).Take(3);

                var matchDetail = new MatchDetails
                {
                    FirstInningBatsman = FirstInningBatsman,
                    SecondInningBatsman = SecondInningBatsman,
                    FirstInningBowler = FirstInningBowler,
                    SecondInningBowler = SecondInningBowler,
                    Team1Score = Team1Score,
                    ProfileUrl = match.ProfileUrl,
                    Team2Score = Team2Score,
                    MatchResult = MatchResult(Team1Score, Team2Score),
                    Ground = match.GroundId.HasValue ? match.Ground.Name : "N/A",
                    Date = match.DateOfMatch,
                    Toss = TossDecide(match, Team1Score, Team2Score),
                    MatchType = match.EventId.HasValue ? match.Event.Name : "Individual/Friendly",
                    FirstInningTop3Batsman = firstInningTop3Batsman.ToList(),
                    SecondInningTop3Batsman = secondInningTop3Batsman.ToList(),
                    FirstInningTop3Bowler = firstInningTop3Bowler.ToList(),
                    SecondInningTop3Bowler = secondInningTop3Bowler.ToList()

                };
                return matchDetail;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private string MatchResult(TeamsScoreDto team1Score, TeamsScoreDto team2Score)
        {
            try
            {
                if (team1Score == null || team2Score == null || team1Score.Score == 0 || team2Score.Score == 0 || team1Score.Score == null || team2Score.Score == null)
                    return "No Result";

                if (team1Score.Score > team2Score.Score)
                {
                    return $"{team1Score.Name} won the match by {team1Score.Score - team2Score.Score}";
                }
                if (team1Score.Score < team2Score.Score)
                {
                    return $"{team2Score.Name} won the match by {10 - team2Score.Wickets}";
                }
                return "Match Tie";
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        private string TossDecide(Match match, TeamsScoreDto team1Score, TeamsScoreDto team2Score)
        {
            try
            {
                if (!match.TossWinningTeam.HasValue)
                    return null;

                var tossWinningTeam = match.TossWinningTeam == match.HomeTeamId ? team1Score.Name : team2Score.Name;

                return match.TossWinningTeam == match.HomeTeamId ? $"{tossWinningTeam} won the toss and decided to bat first" : $"{tossWinningTeam} won the toss and decided to ball first";
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public async Task<FullScoreccard> GetFullScorecard(long team1Id, long team2Id, long matchId)
        {
            try
            {
                var team1Playerscores = await _playerscoreAppService.GetAll(team1Id, matchId);
                var team2Playerscores = await _playerscoreAppService.GetAll(team2Id, matchId);

                var team1FallofWickets = await _fallofWicketAppService.GetByTeamIdAndMatchId(team1Id, matchId);
                var team2FallofWickets = await _fallofWicketAppService.GetByTeamIdAndMatchId(team2Id, matchId);

                var team1Score = await GetByTeamIdAndMatchId(team1Id, matchId);
                var team2Score = await GetByTeamIdAndMatchId(team2Id, matchId);

                var team1 = await _teamAppService.GetById(team1Id);
                var team2 = await _teamAppService.GetById(team2Id);


                var scorecard = new FullScoreccard()
                {
                    Team1FallofWicket = team1FallofWickets,
                    Team2FallofWicket = team2FallofWickets,
                    Team1Playerscore = team1Playerscores,
                    Team2Playerscore = team2Playerscores,
                    Team1Score = team1Score,
                    Team2Score = team2Score,
                    Team1 = team1.Name,
                    Team2 = team2.Name,
                };

                return scorecard;

            }
            catch (Exception e)
            {
                throw e;
            }

        }


        private TeamsScoreDto TeamScoreMapper(List<TeamScore> teamScores, long teamId, List<TeamDto> teams)
        {
            try
            {
                var teamScore = new TeamsScoreDto();
                if (teamScores != null)
                {
                    teamScore = teamScores.Where(i => i.TeamId == teamId).Select(i => new TeamsScoreDto()
                    {
                        Id = i.TeamId,
                        Score = i.TotalScore,
                        Name = i.Team.Name,
                        Overs = i.Overs,
                        Wickets = i.Wickets,
                        Wide = i.Wideballs,
                        Bye = i.Byes,
                        LegBye = i.LegByes,
                        NoBall = i.NoBalls,
                        Extras = i.Wideballs + i.NoBalls + i.Byes + i.LegByes,
                        ProfileUrl = i.Team.ProfileUrl

                    }).FirstOrDefault() ?? new TeamsScoreDto();
                }
                else
                {
                    teamScore = teams.Where(i => i.Id == teamId).Select(i => new TeamsScoreDto()
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Overs = null,
                        Wickets = null,
                        Wide = null,
                        Bye = null,
                        LegBye = null,
                        NoBall = null,
                        Extras = null,
                        ProfileUrl = i.ProfileUrl

                    }).FirstOrDefault() ?? new TeamsScoreDto();
                }
                return teamScore;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private List<MatchBatsman> BatsmanMapper(List<PlayerScore> teamPlayers)
        {
            try
            {
                var FirstInningBatsman = teamPlayers.Select(i => new MatchBatsman()
                {
                    PlayerId = i.PlayerId,
                    PlayerName = i.Player.Name,
                    Bowler = i.Bowler != null ? i.Bowler.Name : "N/A",
                    Fielder = i.Fielder,
                    Runs = i.Bat_Runs,
                    Balls = i.Bat_Balls,
                    Four = i.Four,
                    Six = i.Six,
                    HowOut = i.HowOutId

                }).ToList() ?? new List<MatchBatsman>();
                return FirstInningBatsman;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private List<MatchBowler> BowlersMapper(IEnumerable<PlayerScore> teamBowler)
        {
            try
            {
                var FirstInningBowler = teamBowler.Select(i => new MatchBowler()
                {
                    PlayerId = i.PlayerId,
                    PlayerName = i.Player.Name,
                    Overs = i.Overs,
                    Runs = i.Ball_Runs,
                    Wickets = i.Wickets

                }).ToList() ?? new List<MatchBowler>();
                return FirstInningBowler;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}

