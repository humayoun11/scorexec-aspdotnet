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
using ScoringAppReact.MatchSummary.Dto;
using ScoringAppReact.Teams.Dto;
using Abp.EntityFrameworkCore.Repositories;

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
            if (matchDto.Id == 0 || matchDto.Id == null)
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

        public List<int> CalculateUnorderedStages(int teamCount)
        {
            var unOrderedstages = new List<int>();
            var x = teamCount;
            var s = 0;
            while (x != 1)
            {
                x /= 2;
                s++;
                unOrderedstages.Add(s);
            }

            return unOrderedstages;
        }


        public BracketStages GetAllStagedMatchesByEventId(long eventId)
        {
            var teamCount = _teamRepository.GetAll().Where(i => i.EventId == eventId && i.IsDeleted == false && i.TenantId == _abpSession.TenantId).Count();
            if(teamCount == 0)
            {
                return null;
            }
            var stages = CalculateUnorderedStages(teamCount).OrderBy(i => i).ToList(); ;

            var matches = _repository.GetAll()
                .Include(i => i.HomeTeam)
                .Include(i => i.OppponentTeam)
                .Include(i => i.TeamScores)
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.EventId == eventId);

            var eventMatches = new EventMatches[stages.Count];
            var winTeam = new TeamDto();
            for (var outer = 0; outer < stages.Count; outer++)
            {
                var result = matches
                .Where(i => i.EventStage == stages[outer]).OrderBy(i => i.Id)
                .ToList();

                if (!result.Any())
                    continue;

                var matchLength = result.Count / 2;
                if (result.Count <= 2)
                {
                    matchLength = 1;
                }
                var newMatch = new MatchDto[matchLength];
                var matchIndex = 0;
                var loopLength = eventMatches[outer] != null && eventMatches[outer].Matches.Any() ? eventMatches[outer].Matches.Count : result.Count;

                for (var loop = 0; loop < loopLength; loop++)
                {
                    var currentMatch = eventMatches[outer] != null && eventMatches[outer].Matches.Any() ? eventMatches[outer].Matches[loop] : null;
                    var singleMatch = currentMatch == null ? result[loop] : result.Where(i => i.HomeTeamId == currentMatch.Team1Id &&
                    i.OppponentTeamId == currentMatch.Team2Id).FirstOrDefault();

                    if (singleMatch == null || !singleMatch.TeamScores.Any())
                    {
                        if (loop % 2 == 1)
                            matchIndex++;
                        continue;
                    }

                    var team1Score = singleMatch.TeamScores.Where(i => i.TeamId == singleMatch.HomeTeamId).Select(i => i.TotalScore).FirstOrDefault();
                    var team2Score = singleMatch.TeamScores.Where(i => i.TeamId == singleMatch.OppponentTeamId).Select(i => i.TotalScore).FirstOrDefault();
                    var winningTeam = team1Score > team2Score ? singleMatch.HomeTeam : singleMatch.OppponentTeam;

                    
                    if (outer + 1 == eventMatches.Length)
                    {
                        winTeam.Name = winningTeam.Name;
                        winTeam.Id = winningTeam.Id;
                        continue;
                    }


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
                    eventMatches[outer] = new EventMatches();


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

                if (outer + 1 == eventMatches.Length)
                    continue;

                if (eventMatches[outer + 1] == null)
                    eventMatches[outer + 1] = new EventMatches();

                eventMatches[outer + 1].Matches = newMatch.ToList();

            }
            var data = new BracketStages
            {
                EventMatches = eventMatches.ToList(),
                Winner = winTeam
            };
            return data;
        }

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
                .Include(i => i.TeamScores)
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

        private string MatchResult(ViewMatch match)
        {
            if (match == null || match.Team1Score == 0 || match.Team2Score == 0)
                return "No Result";

            if (match.Team1Score > match.Team2Score)
            {
                return $"{match.Team1} won the match by {match.Team1Score - match.Team2Score}";
            }
            if (match.Team1Score < match.Team2Score)
            {
                return $"{match.Team2} won the match by {10 - match.Team2Wickets}";
            }
            return "Match Tie";
        }

        public List<ViewMatch> GetMOMByPlayerId(long id)
        {
            var result = GetALLMatches().Where(i => i.POM == id).ToList();
            foreach (var item in result)
            {
                item.Result = MatchResult(item);
            }
            return result;
        }

        public List<ViewMatch> GetMatchesByTeamId(long id, int matchResultFilter)
        {
            var result = GetALLMatches().Where(i => (i.Team1Id == id || i.Team2Id == id)).ToList();
            foreach (var item in result)
            {
                item.Result = MatchResult(item);
            }

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
        public List<ViewMatch> GetMatchesByEventId(long id)
        {
            var result = GetALLMatches().Where(i => i.EventId == id).ToList();
            foreach (var item in result)
            {
                item.Result = MatchResult(item);
            }
            return result;
        }


        private List<ViewMatch> GetALLMatches()
        {
            return _repository.GetAll().Where(i => i.IsDeleted == false
               && i.TenantId == _abpSession.TenantId).Select(i => new ViewMatch()
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
                   Mom = i.Player.Name ?? "N/A",
                   EventId = i.EventId,
                   POM = i.PlayerOTM
               }).ToList();
        }

        public void InsertDbRange(List<Match> matches)
        {
            _repository.GetDbContext().AddRange(matches);
        }
    }
}

