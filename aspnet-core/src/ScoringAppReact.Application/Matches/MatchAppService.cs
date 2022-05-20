using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ScoringAppReact.Authorization;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using Abp;
using Abp.Runtime.Session;
using ScoringAppReact.Matches.Dto;
using Abp.UI;
using System;

namespace ScoringAppReact.Matches
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class MatchAppService : AbpServiceBase, IMatchAppService
    {
        private readonly IRepository<Match, long> _repository;
        private readonly IRepository<EventTeam, long> _teamRepository;
        private readonly IAbpSession _abpSession;
        public MatchAppService(IRepository<Match, long> repository, IRepository<EventTeam, long> teamRepository, IAbpSession abpSession)
        {
            _repository = repository;
            _teamRepository = teamRepository;
            _abpSession = abpSession;
        }


        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateMatchDto matchDto)
        {
            ResponseMessageDto result;
            if (matchDto.Id == 0)
            {
                result = await CreateTeamAsync(matchDto);
            }
            else
            {
                result = await UpdateTeamAsync(matchDto);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreateTeamAsync(CreateOrUpdateMatchDto model)
        {
            try
            {
                var result = await _repository.InsertAsync(new Match()
                {
                    GroundId = model.GroundId,
                    MatchOvers = model.MatchOvers,
                    HomeTeamId = model.Team1Id,
                    OppponentTeamId = model.Team2Id,
                    MatchDescription = model.MatchDescription,
                    DateOfMatch = model.DateOfMatch,
                    Season = model.Season,
                    MatchTypeId = model.MatchTypeId,
                    TossWinningTeam = model.TossWinningTeam,
                    PlayerOTM = model.PlayerOTM,
                    EventId = model.EventId,
                    EventStage = model.EventStage,
                    TenantId = _abpSession.TenantId
                });
                await _repository.InsertAsync(result);
                await UnitOfWorkManager.Current.SaveChangesAsync();

                if (result.Id != 0)
                {
                    return new ResponseMessageDto()
                    {
                        Id = result.Id,
                        SuccessMessage = AppConsts.SuccessfullyInserted,
                        Success = true,
                        Error = false,
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
            catch (Exception e)
            {
                throw new UserFriendlyException("No Record Exists", e);
            }

        }

        private async Task<ResponseMessageDto> UpdateTeamAsync(CreateOrUpdateMatchDto model)
        {
            var result = await _repository.UpdateAsync(new Match()
            {
                Id = model.Id.Value,
                GroundId = model.GroundId,
                MatchOvers = model.MatchOvers,
                HomeTeamId = model.Team1Id,
                OppponentTeamId = model.Team2Id,
                MatchDescription = model.MatchDescription,
                DateOfMatch = model.DateOfMatch,
                Season = model.Season,
                MatchTypeId = model.MatchTypeId,
                TossWinningTeam = model.TossWinningTeam,
                PlayerOTM = model.PlayerOTM,
                EventId = model.EventId,
                EventStage = model.EventStage,
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

        public async Task<CreateOrUpdateMatchDto> GetById(long id)
        {
            var result = await _repository.GetAll().Where(i => i.Id == id).Select(i => new CreateOrUpdateMatchDto
            {
                Id = i.Id,
                GroundId = i.GroundId,
                MatchOvers = i.MatchOvers,
                Team1Id = i.HomeTeamId,
                Team2Id = i.OppponentTeamId,
                MatchDescription = i.MatchDescription,
                DateOfMatch = i.DateOfMatch,
                Season = i.Season,
                MatchTypeId = i.MatchTypeId,
                TossWinningTeam = i.TossWinningTeam,
                PlayerOTM = i.PlayerOTM,
                EventId = i.EventId,
                EventStage = i.EventStage,

            })
                .FirstOrDefaultAsync();
            if (result == null)
                throw new UserFriendlyException("No Record Exists");
            return result;
        }

        public async Task<ResponseMessageDto> DeleteAsync(long matchId)
        {

            var model = await _repository.FirstOrDefaultAsync(i => i.Id == matchId);
            if (model == null)
                throw new UserFriendlyException("No Record Exists");
            model.IsDeleted = true;
            var result = await _repository.UpdateAsync(model);

            return new ResponseMessageDto()
            {
                Id = matchId,
                SuccessMessage = AppConsts.SuccessfullyDeleted,
                Success = true,
                Error = false,
            };
        }

        public async Task<List<MatchDto>> GetAll()
        {
            var result = await _repository.GetAll().Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId)
                .Select(i => new MatchDto()
                {
                    Id = i.Id,
                    Ground = i.Ground.Name,
                    Team1 = i.HomeTeam.Name,
                    Team2 = i.OppponentTeam.Name,
                    DateOfMatch = i.DateOfMatch

                }).ToListAsync();
            return result;
        }

        private async Task<List<Match>> GetAllMatches()
        {
            return await _repository.GetAll().Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId).ToListAsync();
        }

        public async Task<List<MatchDto>> GetAllMatchesBYEventId(long eventId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.EventId == eventId)
                .Select(i => new MatchDto()
                {
                    Id = i.Id,
                    Ground = i.Ground.Name,
                    Team1 = i.HomeTeam.Name,
                    Team2 = i.OppponentTeam.Name,
                    DateOfMatch = i.DateOfMatch

                }).ToListAsync();
            return result;
        }


        public List<EventMatches> GetAllStagedMatchesByEventId(long eventId)
        {
            var teamCount = _teamRepository.GetAll().Where(i => i.EventId == eventId).Count();
            var unOrderedstages = new List<int>();
            var x = teamCount;
            while (x != 1)
            {
                x /= 2;
                unOrderedstages.Add(x);
            }
            var matches = _repository.GetAll()
                .Include(i => i.HomeTeam)
                .Include(i => i.OppponentTeam)
                .Include(i => i.TeamScores)
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.EventId == eventId);

            var stages = unOrderedstages.OrderBy(i => i).ToList();
            var eventMatches = new EventMatches[stages.Count];
            for (var outer = 0; outer < stages.Count; outer++)
            {
                var result = matches
                .Where(i => i.EventStage == stages[outer])
                .ToList();

                if (!result.Any())
                    continue;
                var newMatch = new MatchDto[result.Count / 2];
                var matchIndex = 0;
                for (var loop = 0; loop < result.Count; loop++)
                {
                    if (!result[loop].TeamScores.Any())
                        continue;
                    var team1Score = result[loop].TeamScores.Where(i => i.TeamId == result[loop].HomeTeamId).Select(i => i.TotalScore).FirstOrDefault();
                    var team2Score = result[loop].TeamScores.Where(i => i.TeamId == result[loop].OppponentTeamId).Select(i => i.TotalScore).FirstOrDefault();
                    var winningTeam = team1Score > team2Score ? result[loop].HomeTeam : result[loop].OppponentTeam;

                    if (loop % 2 == 0)
                    {
                        if (newMatch[matchIndex] == null)
                        {
                            newMatch[matchIndex] = new MatchDto();
                        }
                        newMatch[matchIndex].Team1 = winningTeam.Name;
                        newMatch[matchIndex].Team1Id = winningTeam.Id;


                    }

                    if (loop % 2 == 1)
                    {
                        if (newMatch[matchIndex] == null)
                        {
                            newMatch[matchIndex] = new MatchDto();
                        }
                        newMatch[matchIndex].Team2 = winningTeam.Name;
                        newMatch[matchIndex].Team2Id = winningTeam.Id;
                        matchIndex++;
                    }
                }

                if (eventMatches[outer] == null)
                {
                    eventMatches[outer] = new EventMatches();
                }

                if (eventMatches[outer].Matches == null)
                {
                    eventMatches[outer].Matches = result
                      .Select(i => new MatchDto()
                      {
                          Id = i.Id,
                          MatchOvers = i.MatchOvers,
                          Team1Id = i.HomeTeamId,
                          Team2Id = i.OppponentTeamId,
                          Team1 = i.HomeTeam.Name,
                          Team2 = i.OppponentTeam.Name,
                          MatchDescription = i.MatchDescription,
                          DateOfMatch = i.DateOfMatch,
                          Season = i.Season,
                          EventId = i.EventId
                      }).ToList();
                }
                else
                {
                    foreach (var item in result)
                    {
                        var index = eventMatches[outer].Matches.FindIndex(i => i.Team1Id == item.HomeTeamId && i.Team2Id == item.OppponentTeamId);
                        eventMatches[outer].Matches[index] = result.Where(i => i.HomeTeamId == item.HomeTeamId && i.OppponentTeamId == item.OppponentTeamId)
                      .Select(i => new MatchDto()
                      {
                          Id = i.Id,
                          MatchOvers = i.MatchOvers,
                          Team1Id = i.HomeTeamId,
                          Team2Id = i.OppponentTeamId,
                          Team1 = i.HomeTeam.Name,
                          Team2 = i.OppponentTeam.Name,
                          MatchDescription = i.MatchDescription,
                          DateOfMatch = i.DateOfMatch,
                          Season = i.Season,
                          EventId = i.EventId
                      }).SingleOrDefault();
                    }
                }


                if (eventMatches[outer + 1] == null)
                {
                    eventMatches[outer + 1] = new EventMatches();
                }

                eventMatches[outer + 1].Matches = newMatch.ToList();

            }
            return eventMatches.ToList();
        }

        public async Task<List<BracketStages>> GetTeamsOfStage(int eventId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.EventId == eventId)
                .Include(i => i.HomeTeam)
                .Include(i => i.OppponentTeam)
                .Include(i => i.TeamScores).OrderByDescending(i => i.Id).ToListAsync();

            var teamCount = _teamRepository.GetAll().Where(i => i.EventId == eventId).Count();

            var stages = new List<int>();
            var x = teamCount;
            while (x != 1)
            {
                x /= 2;
                stages.Add(x);
            }

            stages.Sort();
            var bracketStage = new List<BracketStages>();
            foreach (var stage in stages)
            {
                var teams = new List<StageTeams>();
                foreach (var item in result.Where(i => i.EventStage == stage))
                {
                    if (!item.TeamScores.Any())
                        continue;
                    var team1Score = item.TeamScores.Where(i => i.TeamId == item.HomeTeamId).Select(i => i.TotalScore).FirstOrDefault();
                    var team2Score = item.TeamScores.Where(i => i.TeamId == item.OppponentTeamId).Select(i => i.TotalScore).FirstOrDefault();
                    teams.Add(new StageTeams
                    {
                        TeamId = team1Score > team2Score ? item.HomeTeamId : item.OppponentTeamId,
                        TeamName = team1Score > team2Score ? item.HomeTeam.Name : item.OppponentTeam.Name,
                        Date = item.DateOfMatch
                    });
                }
                bracketStage.Add(new BracketStages
                {
                    StageTeams = teams
                });
            }
            return bracketStage;
        }


        //public async Task<List<BracketStages>> GetStageMatches(int eventId)
        //{
        //    var result = await _repository.GetAll()
        //        .Where(i => i.IsDeleted == false && i.EventId == eventId)
        //        .Include(i => i.HomeTeam)
        //        .Include(i => i.OppponentTeam)
        //        .Include(i => i.TeamScores).ToListAsync();

        //    var teamCount = _teamRepository.GetAll().Where(i => i.EventId == eventId).Count();

        //    var stages = new List<int>();
        //    var x = teamCount;
        //    while (x != 1)
        //    {
        //        x /= 2;
        //        stages.Add(x);
        //    }

        //    stages.Sort();
        //    var bracketStage = new List<BracketStages>();
        //    foreach (var stage in stages)
        //    {
        //        var teams = new List<StageTeams>();
        //        foreach (var item in result.Where(i => i.EventStage == stage))
        //        {
        //            if (!item.TeamScores.Any())
        //                continue;
        //            var team1Score = item.TeamScores.Where(i => i.TeamId == item.HomeTeamId).Select(i => i.TotalScore).FirstOrDefault();
        //            var team2Score = item.TeamScores.Where(i => i.TeamId == item.OppponentTeamId).Select(i => i.TotalScore).FirstOrDefault();
        //            teams.Add(new StageTeams
        //            {
        //                Team1Id = team1Score > team2Score ? item.HomeTeamId : item.OppponentTeamId,
        //                Team2Name = team1Score > team2Score ? item.HomeTeam.Name : item.OppponentTeam.Name,
        //                Date = item.DateOfMatch
        //            });
        //        }
        //        bracketStage.Add(new BracketStages
        //        {
        //            StageTeams = teams
        //        });
        //    }
        //    return bracketStage;
        //}

        public async Task<PagedResultDto<MatchDto>> GetPaginatedAllAsync(PagedMatchResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (i.TenantId == _abpSession.TenantId) &&
                   (!input.Team1Id.HasValue || i.HomeTeamId == input.Team1Id || i.OppponentTeamId == input.Team1Id) && (!input.Team2Id.HasValue || i.HomeTeamId == input.Team2Id || i.OppponentTeamId == input.Team2Id))
                .WhereIf(input.Overs.HasValue, i => i.MatchOvers == input.Overs)
                .WhereIf(input.Type.HasValue, i => i.MatchTypeId == input.Type)
                .WhereIf(input.Date.HasValue, i => i.DateOfMatch == input.Date)
                .WhereIf(input.GroundId.HasValue, i => i.GroundId == input.GroundId);

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderByDescending(i => i.Id)
                .PageBy(input);

            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<MatchDto>(
                totalCount: totalCount,
                items: await pagedAndFilteredPlayers.Select(i => new MatchDto()
                {
                    Id = i.Id,
                    Ground = i.Ground.Name,
                    Team1 = i.HomeTeam.Name,
                    Team2 = i.OppponentTeam.Name,
                    DateOfMatch = i.DateOfMatch,
                    MatchType = i.MatchTypeId.ToString(),
                    Team1Id = i.HomeTeamId,
                    Team2Id = i.OppponentTeamId,
                    MatchOvers = i.MatchOvers,
                    EventName = i.Event.Name ?? "N/A"
                }).ToListAsync());
        }

        public async Task<EventMatch> EditEventMatch(long id)
        {
            var result = await _repository.GetAll().Where(i => i.Id == id).Select(i => new EventMatch
            {
                Id = i.Id,
                GroundId = i.GroundId,
                Ground = i.Ground.Name,
                MatchOvers = i.MatchOvers,
                Team1Id = i.HomeTeamId,
                Team1 = i.HomeTeam.Name,
                Team2Id = i.OppponentTeamId,
                Team2 = i.OppponentTeam.Name,
                MatchDescription = i.MatchDescription,
                DateOfMatch = i.DateOfMatch,
                Season = i.Season,
                MatchTypeId = i.MatchTypeId,
                TossWinningTeam = i.TossWinningTeam,
                PlayerOTM = i.PlayerOTM,
                EventId = i.EventId,
                Event = i.Event.Name,
                EventStage = i.EventStage,

            })
                .FirstOrDefaultAsync();
            if (result == null)
                throw new UserFriendlyException("No Record Exists");
            return result;
        }


        public async Task<List<ViewMatch>> GetMatchesByPlayerId(long id, int matchResultFilter)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false
                    && i.TenantId == _abpSession.TenantId
                    && i.PlayerScores.Any(j => j.PlayerId == id))
                .Select(i => new ViewMatch()
                {
                    Id = i.Id,
                    Ground = i.Ground.Name,
                    Date = i.DateOfMatch,
                    Team1 = i.HomeTeam.Name,
                    Team1Id = i.HomeTeamId,
                    Team2 = i.OppponentTeam.Name,
                    Team2Id = i.OppponentTeamId,
                    Team1Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team2Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team1Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team2Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team1Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    Team2Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    MatchType = i.MatchTypeId == 1 ? "Friendly" : i.MatchTypeId == 2 ? "Tournament" : "Series",
                    Tournament = i.MatchTypeId == 2 ? $"Tournament: {i.Event.Name}"
                    : "Friendly / Individual",
                    Mom = i.Player.Name ?? "N/A"
                })

                .ToListAsync();

            if (matchResultFilter == 2)
            {
                return result.Where(i => i.Team1Id == id ? i.Team1Score > i.Team2Score : i.Team2Score > i.Team1Score).ToList();

            }
            else
            if (matchResultFilter == 3)
            {
                return result.Where(i => i.Team1Id == id ? i.Team1Score < i.Team2Score : i.Team2Score < i.Team1Score).ToList();

            }
            else if (matchResultFilter == 4)
            {
                return result.Where(i => i.Team1Score == i.Team2Score).ToList();
            }
            return result;
        }

        public async Task<List<ViewMatch>> GetMOMByPlayerId(long id)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false
                    && i.TenantId == _abpSession.TenantId
                    && i.PlayerOTM == id)
                .Select(i => new ViewMatch()
                {
                    Id = i.Id,
                    Ground = i.Ground.Name,
                    Date = i.DateOfMatch,
                    Team1 = i.HomeTeam.Name,
                    Team1Id = i.HomeTeamId,
                    Team2 = i.OppponentTeam.Name,
                    Team2Id = i.OppponentTeamId,
                    Team1Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team2Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team1Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team2Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team1Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    Team2Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    MatchType = i.MatchTypeId == 1 ? "Friendly" : i.MatchTypeId == 2 ? "Tournament" : "Series",
                    Tournament = i.MatchTypeId == 2 ? $"Tournament: {i.Event.Name}"
                    : "Friendly / Individual",
                    Mom = i.Player.Name ?? "N/A"
                })

                .ToListAsync();
            return result;
        }

        public async Task<List<ViewMatch>> GetMatchesByTeamId(long id, int matchResultFilter)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false
                    && i.TenantId == _abpSession.TenantId
                    && (i.HomeTeamId == id || i.OppponentTeamId == id))
                .Select(i => new ViewMatch()
                {
                    Id = i.Id,
                    Ground = i.Ground.Name,
                    Date = i.DateOfMatch,
                    Team1 = i.HomeTeam.Name,
                    Team1Id = i.HomeTeamId,
                    Team2 = i.OppponentTeam.Name,
                    Team2Id = i.OppponentTeamId,
                    Team1Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team2Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team1Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team2Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team1Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    Team2Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    MatchType = i.MatchTypeId == 1 ? "Friendly" : i.MatchTypeId == 2 ? "Tournament" : "Series",
                    Tournament = i.MatchTypeId == 2 ? $"Tournament: {i.Event.Name}"
                    : "Friendly / Individual",
                    Mom = i.Player.Name ?? "N/A"
                })

                .ToListAsync();


            if (matchResultFilter == 2)
            {
                return result.Where(i => i.Team1Id == id ? i.Team1Score > i.Team2Score : i.Team2Score > i.Team1Score).ToList();

            }
            else
            if (matchResultFilter == 3)
            {
                return result.Where(i => i.Team1Id == id ? i.Team1Score < i.Team2Score : i.Team2Score < i.Team1Score).ToList();

            }
            else if (matchResultFilter == 4)
            {
                return result.Where(i => i.Team1Score == i.Team2Score).ToList();
            }
            return result;
        }
        public async Task<List<ViewMatch>> GetMatchesByEventId(long id)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false
                && i.TenantId == _abpSession.TenantId
                && i.EventId == id)
                .Select(i => new ViewMatch()
                {
                    Id = i.Id,
                    Ground = i.Ground.Name,
                    Date = i.DateOfMatch,
                    Team1 = i.HomeTeam.Name,
                    Team1Id = i.HomeTeamId,
                    Team2 = i.OppponentTeam.Name,
                    Team2Id = i.OppponentTeamId,
                    Team1Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team2Score = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.TotalScore).SingleOrDefault(),
                    Team1Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team2Overs = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Overs).SingleOrDefault(),
                    Team1Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.HomeTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    Team2Wickets = i.TeamScores.Where(j => j.MatchId == i.Id && j.TeamId == i.OppponentTeamId).Select(j => j.Wickets).SingleOrDefault(),
                    MatchType = i.MatchTypeId == 1 ? "Friendly" : i.MatchTypeId == 2 ? "Tournament" : "Series",
                    Tournament = i.MatchTypeId == 2 ? $"Tournament: {i.Event.Name}"
                    : "Friendly / Individual",
                    Mom = i.Player.Name ?? "N/A"
                }).ToListAsync();
            return result;
        }
    }
}

