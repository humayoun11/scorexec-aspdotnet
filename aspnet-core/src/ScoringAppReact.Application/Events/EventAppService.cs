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

namespace ScoringAppReact.Events
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class EventAppService : AbpServiceBase, IEventAppService
    {
        private readonly IRepository<Event, long> _repository;
        private readonly IAbpSession _abpSession;
        public EventAppService(IRepository<Event, long> repository, IAbpSession abpSession)
        {
            _repository = repository;
            _abpSession = abpSession;
        }


        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateEventDto eventDto)
        {
            ResponseMessageDto result;
            if (eventDto.Id == 0)
            {
                result = await CreateEventAsync(eventDto);
            }
            else
            {
                result = await UpdateEventAsync(eventDto);
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

        public async Task<EventDto> GetById(long eventId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.Id == eventId)
                .Select(i =>
                new EventDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    FileName = i.FileName,
                })
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<ResponseMessageDto> DeleteAsync(long playerId)
        {
            var model = await _repository.GetAll().Where(i => i.Id == playerId).FirstOrDefaultAsync();
            model.IsDeleted = true;
            var result = await _repository.UpdateAsync(model);

            return new ResponseMessageDto()
            {
                Id = playerId,
                SuccessMessage = AppConsts.SuccessfullyDeleted,
                Success = true,
                Error = false,
            };
        }

        public async Task<List<EventDto>> GetAll(long? tenantId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId)
                .Select(i => new EventDto()
                {
                    Id = i.Id,
                    Name = i.Name,

                }).ToListAsync();
            return result;
        }

        public async Task<PagedResultDto<EventDto>> GetPaginatedAllAsync(PagedEventResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (!input.TenantId.HasValue || i.TenantId == input.TenantId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                    x => x.Name.Contains(input.Name));

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderBy(i => i.Name)
                .PageBy(input);

            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<EventDto>(
                totalCount: totalCount,
                items: await pagedAndFilteredPlayers.Select(i => new EventDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    FileName = i.FileName
                }).ToListAsync());
        }
    }
}

