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
using ScoringAppReact.Events.Dto;
using System;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using ScoringAppReact.EntityFrameworkCore;
using Dapper;
using System.Data;
using ScoringAppReact.Matches;
using ScoringAppReact.Teams;
using ScoringAppReact.Matches.Dto;
using Abp.UI;
using ScoringAppReact.PictureGallery;
using ScoringAppReact.Teams.Dto;

namespace ScoringAppReact.Events
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class EventAppService : AbpServiceBase, IEventAppService
    {
        private readonly IRepository<Event, long> _repository;
        private readonly IRepository<EventTeam, long> _eventTeamRepository;
        private readonly IRepository<Team, long> _teamRepository;
        private readonly IAbpSession _abpSession;
        private readonly MatchAppService _matchAppService;
        private readonly IDbContextProvider<ScoringAppReactDbContext> _context;
        private readonly PictureGalleryAppService _pictureGalleryAppService;
        public EventAppService(IRepository<Event, long> repository,
            IRepository<EventTeam, long> eventTeamRepository,
            IRepository<Team, long> teamRepository,
            IAbpSession abpSession,
            MatchAppService matchAppService,
            PictureGalleryAppService pictureGalleryAppService,
            IDbContextProvider<ScoringAppReactDbContext> context)
        {
            _repository = repository;
            _abpSession = abpSession;
            _eventTeamRepository = eventTeamRepository;
            _context = context;
            _matchAppService = matchAppService;
            _teamRepository = teamRepository;
            _pictureGalleryAppService = pictureGalleryAppService;
        }


        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateEventDto model)
        {
            ResponseMessageDto result;
            if (model.Id == 0)
            {
                result = await CreateEventAsync(model);
            }
            else
            {
                result = await UpdateEventAsync(model);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreateEventAsync(CreateOrUpdateEventDto model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new UserFriendlyException("Name must required");
                //return;
            }
                
            if (model.Profile != null)
            {
                if (string.IsNullOrEmpty(model.Profile.Url))
                {
                    var profilePicture = _pictureGalleryAppService.GetImageUrl(model.Profile);
                    model.ProfileUrl = profilePicture.Url;
                }
            }


            var result = await _repository.InsertAsync(new Event()
            {
                Name = model.Name,
                ProfileUrl = model.ProfileUrl,
                Organizor = model.Organizor,
                OrganizorContact = model.OrganizorContact,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                EventType = model.EventType,
                TournamentType = model.TournamentType,
                NumberOfGroup = model.NumberOfGroup,
                TenantId = _abpSession.TenantId

            });
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (model.Gallery != null)
            {
                var gallery = new CreateOrUpdateGalleryDto
                {
                    TeamId = result.Id,
                    Galleries = model.Gallery
                };

                await _pictureGalleryAppService.CreateAsync(gallery);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }

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

        private async Task<ResponseMessageDto> UpdateEventAsync(CreateOrUpdateEventDto model)
        {
            if (model.Profile != null)
            {
                if (string.IsNullOrEmpty(model.Profile.Url))
                {
                    var profilePicture = _pictureGalleryAppService.GetImageUrl(model.Profile);
                    model.ProfileUrl = profilePicture.Url;
                }
            }


            var result = await _repository.UpdateAsync(new Event()
            {
                Id = model.Id.Value,
                Name = model.Name,
                ProfileUrl = model.ProfileUrl,
                Organizor = model.Organizor,
                OrganizorContact = model.OrganizorContact,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                EventType = model.EventType,
                TournamentType = model.TournamentType,
                NumberOfGroup = model.NumberOfGroup,
                TenantId = _abpSession.TenantId

            });
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (model.Gallery != null)
            {
                var gallery = new CreateOrUpdateGalleryDto
                {
                    TeamId = result.Id,
                    Galleries = model.Gallery
                };

                await _pictureGalleryAppService.UpdateAsync(gallery);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }

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

        public async Task<ResponseMessageDto> CreateOrUpdateEventTeamsAsync(EventTeamDto model)
        {
            var allEventTeams = _eventTeamRepository.GetAll().Where(i => i.EventId == model.EventId && i.IsDeleted == false).ToList();
            var prev = allEventTeams.Select(i => i.TeamId);
            var toDelete = prev.Except(model.TeamIds);
            var toAddNew = model.TeamIds.Except(prev);
            if (toDelete.Any())
            {
                var deleteTeams = new List<EventTeam>();
                foreach (var id in toDelete)
                {
                    var team = allEventTeams.Where(j => j.TeamId == id).FirstOrDefault();
                    team.IsDeleted = true;
                    deleteTeams.Add(team);
                }
                _eventTeamRepository.GetDbContext().UpdateRange(deleteTeams);
            }
            if (toAddNew.ToList().Any())
            {
                var addNewTeams = new List<EventTeam>();
                foreach (var id in toAddNew)
                {
                    var team = new EventTeam()
                    {
                        EventId = model.EventId,
                        TeamId = id,
                        TenantId = _abpSession.TenantId
                    };
                    addNewTeams.Add(team);
                }
                _eventTeamRepository.GetDbContext().AddRange(addNewTeams);
            }

            await UnitOfWorkManager.Current.SaveChangesAsync();

            var teams = _eventTeamRepository.GetAll().Where(i => i.EventId == model.EventId && i.IsDeleted == false).ToList();

            _matchAppService.CreateBracketMatch(teams,model.EventId);


            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (model.EventId != 0)
            {
                return new ResponseMessageDto()
                {
                    Id = model.EventId,
                    SuccessMessage = AppConsts.SuccessfullyUpdated,
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

        public async Task<EventDto> GetById(long id)
        {
            if (id == 0)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var result = await _repository.GetAll()
                .Where(i => i.Id == id)
                .Select(i =>
                new EventDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    EventType = i.EventType,
                    TournamentType = i.TournamentType,
                    NumberOfGroup = i.NumberOfGroup,
                    ProfileUrl = i.ProfileUrl
                })
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<ResponseMessageDto> DeleteAsync(long id)
        {
            var model = await _repository.GetAll().Where(i => i.Id == id).FirstOrDefaultAsync();
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

        public async Task<List<EventDto>> GetAll()
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId)
                .Select(i => new EventDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    EventType = i.EventType,
                    TournamentType = i.TournamentType,
                    NumberOfGroup = i.NumberOfGroup
                }).ToListAsync();
            return result;
        }

        public async Task<PagedResultDto<EventDto>> GetPaginatedAllAsync(PagedEventResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (i.TenantId == _abpSession.TenantId)
                && (!input.Type.HasValue || i.EventType == input.Type) && (!input.StartDate.HasValue || i.StartDate >= input.StartDate)
                && (!input.EndDate.HasValue || i.EndDate <= input.EndDate))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                    x => x.Name.ToLower().Contains(input.Name.ToLower()));

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderByDescending(i => i.Id)
                .PageBy(input);

            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<EventDto>(
                totalCount: totalCount,
                items: await pagedAndFilteredPlayers.Select(i => new EventDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    ProfileUrl = i.ProfileUrl,
                    EventType = i.EventType,
                    TournamentType = i.TournamentType,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    NumberOfGroup = i.NumberOfGroup
                }).ToListAsync());
        }

        public async Task<List<EventDto>> GetAllEventsByTeamId(long id, int? typeId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false &&
                i.TenantId == _abpSession.TenantId &&
                i.EventTeams.Any(j => j.TeamId == id) && (!typeId.HasValue || i.EventType == typeId))
                .Select(i => new EventDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    EventType = i.EventType,
                    TournamentType = i.TournamentType,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    Organizor = i.Organizor,
                    OrganizorContact = i.OrganizorContact
                }).ToListAsync();
            return result;
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<EventStats> GetEventStat(long id)
        {
            try
            {
                var dbContext = _context.GetDbContext();
                var connection = dbContext.Database.GetDbConnection();
                var paramEventId = id;
                var result = await connection.QueryFirstOrDefaultAsync<EventStats>("usp_GetEventStatistics",
                    new { paramEventId },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    var stats = new EventStats();
                    var e = await GetById(id);
                    stats.Event = e.Name;
                    stats.Organizor = e.Organizor;
                    stats.Groups = e.NumberOfGroup;
                    stats.Type = e.TournamentType;
                    return stats;
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public ResponseMessageDto AddGroupWiseEventTeam(CreateGroupWiseTeams model)
        {
            var Allteams = _eventTeamRepository.GetAll().Where(i => i.EventId == model.EventId && i.IsDeleted == false && i.TenantId == _abpSession.TenantId).ToList();
            var group = 1;
            foreach (var selectedTeam in model.SelectedTeams)
            {
                var oldTeamIds = Allteams.Where(i => i.Group == group).Select(i => i.TeamId).ToList();
                var toAddTeams = selectedTeam.Except(oldTeamIds).ToList();
                var ToDelete = oldTeamIds.Except(selectedTeam).ToList();
                var ListEventTeam = new List<EventTeam>();
                if (toAddTeams.Any())
                {
                    foreach (var item in toAddTeams)
                    {
                        var EventTeam = new EventTeam
                        {
                            EventId = model.EventId,
                            TeamId = item,
                            Group = group,
                            TenantId = _abpSession.TenantId
                        };
                        ListEventTeam.Add(EventTeam);
                    }
                    _eventTeamRepository.GetDbContext().UpdateRange(ListEventTeam);
                }


                if (ToDelete.Any())
                {
                    foreach (var item in ToDelete)
                    {
                        var deleteTeam = Allteams.Where(i => i.TeamId == item).SingleOrDefault();
                        _eventTeamRepository.Delete(deleteTeam);

                    }
                }



                CurrentUnitOfWork.SaveChanges();
                group++;

            }


            if (--group == model.SelectedTeams.Count)
            {
                return new ResponseMessageDto()
                {
                    Id = group,
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



        public async Task<GroupWisePointTable[]> GetPointsTable(long id)
        {
            var thisEvent = await GetById(id);
            if (thisEvent.NumberOfGroup.HasValue && thisEvent.NumberOfGroup > 0)
            {
                var GroupWisePointsTable = new GroupWisePointTable[thisEvent.NumberOfGroup.Value];
                var matches = _matchAppService.GetMatchesByEventId(id);
                var teams = await _teamRepository.GetAll().Include(i => i.EventTeams).Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.EventTeams.Any(j => j.EventId == id)).ToListAsync();
                if (matches == null || matches.Count == 0 || teams == null || teams.Count == 0)
                {
                    return GroupWisePointsTable;
                }

                for (var index = 1; index <= thisEvent.NumberOfGroup; index++)
                {
                    var pointsTable = new List<PointsTableDto>();

                    var groupTeams = teams.Where(i => i.EventTeams.Any(j => j.Group == index)).ToList();

                    foreach (var team in groupTeams)
                    {
                        var winningMatch = new List<ViewMatch>();
                        var loosingMatch = new List<ViewMatch>();
                        var tieMatch = new List<ViewMatch>();
                        var noResult = new List<ViewMatch>();

                        var myMatches = matches.Where(i => i.Team1Id == team.Id || i.Team2Id == team.Id).ToList();

                        var points = 0;
                        foreach (var match in myMatches)
                        {

                            if (match.Team1Score is 0 || match.Team2Score is 0)
                            {
                                noResult.Add(match);
                            }
                            else if (match.Team1Id == team.Id && match.Team1Score > match.Team2Score)
                            {
                                winningMatch.Add(match);
                                points += 2;
                            }
                            else if (match.Team2Id == team.Id && match.Team1Score > match.Team2Score)
                            {
                                loosingMatch.Add(match);
                            }
                            else if (match.Team1Id == team.Id && match.Team1Score < match.Team2Score)
                            {
                                loosingMatch.Add(match);
                            }
                            else if (match.Team2Id == team.Id && match.Team1Score < match.Team2Score)
                            {
                                winningMatch.Add(match);
                                points += 2;
                            }
                            else if (match.Team1Score == match.Team2Score)
                            {
                                tieMatch.Add(match);
                                points++;
                            }
                            else
                            {
                                noResult.Add(match);
                            }
                        }

                        var item = new PointsTableDto
                        {
                            Team = team.Name,
                            Played = myMatches.Count(),
                            Won = winningMatch.Count(),
                            Lost = loosingMatch.Count(),
                            Tie = tieMatch.Count(),
                            Points = points,
                            RunRate = 0
                        };
                        pointsTable.Add(item);
                    }
                    if (GroupWisePointsTable[index - 1] == null)
                    {
                        GroupWisePointsTable[index - 1] = new GroupWisePointTable();
                    }

                    GroupWisePointsTable[index - 1].PointsTables = pointsTable;
                }

                return GroupWisePointsTable;
            }


            return null;
        }
    }
}

