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
using Abp.UI;
using ScoringAppReact.TeamScores.Dto;
using ScoringAppReact.MatchSummary.Dto;
using ScoringAppReact.PlayerScores;
using ScoringAppReact.FallOfWickets;
using System;
using ScoringAppReact.Teams;

namespace ScoringAppReact.TeamScores
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class TeamScoresAppService : AbpServiceBase, ITeamScoresAppService
    {
        private readonly IRepository<TeamScore, long> _repository;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<PlayerScore, long> _playerScoreRepository;
        private readonly IRepository<Match, long> _matchRepository;
        private readonly PlayerScoreAppService _playerscoreAppService;
        private readonly FallofWicketAppService _fallofWicketAppService;
        private readonly TeamAppService _teamAppService;

        public TeamScoresAppService(IRepository<TeamScore, long> repository,
            IRepository<Match, long> matchRepository, IAbpSession abpSession,
            IRepository<PlayerScore, long> playerScoreRepository,
            PlayerScoreAppService playerscoreAppService,
            FallofWicketAppService fallofWicketAppService,
            TeamAppService teamAppService
            )
        {
            _repository = repository;
            _abpSession = abpSession;
            _playerScoreRepository = playerScoreRepository;
            _matchRepository = matchRepository;
            _playerscoreAppService = playerscoreAppService;
            _fallofWicketAppService = fallofWicketAppService;
            _teamAppService = teamAppService;
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
            var alreadyAdded = await _repository.FirstOrDefaultAsync(i => i.MatchId == model.MatchId && i.TeamId == model.TeamId && i.IsDeleted == false);
            if (alreadyAdded != null)
            {
                throw new UserFriendlyException("Already added with associated team and match");
            }


            var result = await _repository.InsertAsync(new TeamScore()
            {
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
            var result = await _repository.UpdateAsync(new TeamScore()
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
            var result = await _repository.GetAll()
                .FirstOrDefaultAsync(i => i.Id == id);
            return ObjectMapper.Map<TeamScoreDto>(result);
        }

        public async Task<ResponseMessageDto> DeleteAsync(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Id id required");
                //return;
            }
            var model = await _repository.FirstOrDefaultAsync(i => i.Id == id);

            if (model == null)
            {
                throw new UserFriendlyException("No record found with associated Id");
                //return;
            }
            model.IsDeleted = true;
            var result = await _repository.UpdateAsync(model);

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
            var result = await _repository.GetAll().Select(j => new TeamScoreDto()
            {
                Id = j.Id,
                TotalScore = j.TotalScore,
                Byes = j.Byes,
                LegByes = j.LegByes,
                NoBalls = j.NoBalls,
                Wideballs = j.Wideballs,
                Overs = j.Overs,
                Wickets = j.Wickets,
                TeamId = j.TeamId,
                MatchId = j.MatchId,
                TenantId = j.TenantId
            }).FirstOrDefaultAsync(i => i.TeamId == teamId && i.MatchId == matchId && i.TenantId == _abpSession.TenantId);

            return result;
        }
        public async Task<MatchDetails> GetTeamScorecard(long team1Id, long team2Id, long matchId)
        {
            var playerScore = await _playerScoreRepository.GetAll()
                .Include(i => i.Player)
                .Include(i => i.Bowler)
                .Where(i => i.MatchId == matchId && i.IsDeleted == false).ToListAsync();
            var teamScores = await _repository.GetAll()
                .Include(i => i.Team)
                .Where(i => i.MatchId == matchId && i.IsDeleted == false).ToListAsync();
            var match = await _matchRepository.GetAll()
                .Include(i => i.Ground)
                .Include(i => i.Event)
                .Where(i => i.Id == matchId).FirstOrDefaultAsync();

            var team1Players = playerScore.Where(i => i.TeamId == team1Id).OrderBy(i => i.Position).ToList();
            var team2Players = playerScore.Where(i => i.TeamId == team2Id).OrderBy(i => i.Position).ToList();

            var team1Bowler = team1Players.Where(i => i.Overs.HasValue);
            var team2Bowler = team2Players.Where(i => i.Overs.HasValue);

            var FirstInningBatsman = team1Players.Select(i => new MatchBatsman()
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

            var SecondInningBatsman = team2Players.Select(i => new MatchBatsman()
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

            var FirstInningBowler = team2Bowler.Select(i => new MatchBowler()
            {
                PlayerId = i.PlayerId,
                PlayerName = i.Player.Name,
                Overs = i.Overs,
                Runs = i.Ball_Runs,
                Wickets = i.Wickets

            }).ToList() ?? new List<MatchBowler>();

            var SecondInningBowler = team1Bowler.Select(i => new MatchBowler()
            {
                PlayerId = i.PlayerId,
                PlayerName = i.Player.Name,
                Overs = i.Overs,
                Runs = i.Ball_Runs,
                Wickets = i.Wickets

            }).ToList() ?? new List<MatchBowler>();

            var Team1Score = teamScores.Where(i => i.TeamId == team1Id).Select(i => new TeamsScoreDto()
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
                Extras = i.Wideballs + i.NoBalls + i.Byes + i.LegByes

            }).FirstOrDefault() ?? new TeamsScoreDto();

            var Team2Score = teamScores.Where(i => i.TeamId == team2Id).Select(i => new TeamsScoreDto()
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
                Extras = i.Wideballs + i.NoBalls + i.Byes + i.LegByes

            }).FirstOrDefault() ?? new TeamsScoreDto();

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

        private string MatchResult(TeamsScoreDto team1Score, TeamsScoreDto team2Score)
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


        private string TossDecide(Match match, TeamsScoreDto team1Score, TeamsScoreDto team2Score)
        {
            if (!match.TossWinningTeam.HasValue)
                return null;

            var tossWinningTeam = match.TossWinningTeam == match.HomeTeamId ? team1Score.Name : team2Score.Name;

            return match.TossWinningTeam == match.HomeTeamId ? $"{tossWinningTeam} won the toss and decided to bat first" : $"{tossWinningTeam} won the toss and decided to ball first";
        }

        public async Task<FullScoreccard> getFullScorecard(long team1Id, long team2Id, long matchId)
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
    }
}

