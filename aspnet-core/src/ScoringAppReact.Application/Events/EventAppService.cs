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

namespace ScoringAppReact.Events
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class EventAppService : AbpServiceBase, IEventAppService
    {
        private readonly IRepository<Event, long> _repository;
        private readonly IRepository<Match, long> _matchRepository;
        private readonly IRepository<EventTeam, long> _eventTeamRepository;
        private readonly IAbpSession _abpSession;
        public EventAppService(IRepository<Event, long> repository,
            IRepository<EventTeam, long> eventTeamRepository,
            IRepository<Match, long> matchRepository,
            IAbpSession abpSession)
        {
            _repository = repository;
            _abpSession = abpSession;
            _eventTeamRepository = eventTeamRepository;
            _matchRepository = matchRepository;
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
            var result = await _repository.InsertAsync(new Event()
            {
                Name = model.Name,
                FileName = model.FileName,
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
            var result = await _repository.UpdateAsync(new Event()
            {
                Id = model.Id.Value,
                Name = model.Name,
                FileName = model.FileName,
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
            if (model.IsCreateMatch)
            {
                var teams = _eventTeamRepository.GetAll().Where(i => i.EventId == model.EventId && i.IsDeleted == false).ToList();
                var matchList = new List<Match>();
                for (var i = 0; i < teams.Count; i += 2)
                {
                    matchList.Add(new Match()
                    {
                        HomeTeamId = teams[i].TeamId,
                        OppponentTeamId = teams[i+1].TeamId,
                        MatchTypeId = MatchTypeConsts.Tournament,
                        EventId = model.EventId,
                        EventStage = EventStageConsts.Group,
                        TenantId = _abpSession.TenantId
                    });
                }
                _matchRepository.GetDbContext().AddRange(matchList);

            }


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
                    TournamentType = i.TournamentType
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
                    EventType = i.EventType
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
                    FileName = i.FileName,
                    EventType = i.EventType,
                    TournamentType = i.TournamentType,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate
                }).ToListAsync());
        }
    }
}

