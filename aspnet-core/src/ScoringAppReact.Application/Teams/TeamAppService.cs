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
using ScoringAppReact.Teams.Dto;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Abp.UI;
using System;
using ScoringAppReact.Events.Dto;

namespace ScoringAppReact.Teams
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class TeamAppService : AbpServiceBase, ITeamAppService
    {
        private readonly IRepository<Team, long> _repository;
        private readonly IAbpSession _abpSession;
        public TeamAppService(IRepository<Team, long> repository, IAbpSession abpSession)
        {
            _repository = repository;
            _abpSession = abpSession;
        }


        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateTeamDto teamDto)
        {
            ResponseMessageDto result;
            if (teamDto.Id == 0)
            {
                result = await CreateTeamAsync(teamDto);
            }
            else
            {
                result = await UpdateTeamAsync(teamDto);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreateTeamAsync(CreateOrUpdateTeamDto model)
        {


            var result = await _repository.InsertAsync(new Team()
            {
                Name = model.Name,
                Place = model.Place,
                Zone = model.Zone,
                Contact = model.Contact,
                IsRegistered = model.IsRegistered,
                City = model.City,
                FileName = model.FileName,
                Type = model.Type,
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

        private async Task<ResponseMessageDto> UpdateTeamAsync(CreateOrUpdateTeamDto teamDto)
        {
            var result = await _repository.UpdateAsync(new Team()
            {
                Id= teamDto.Id.Value,
                Name = teamDto.Name,
                Contact = teamDto.Contact,
                FileName = teamDto.FileName,
                Zone = teamDto.Zone,
                IsRegistered = teamDto.IsRegistered,
                City = teamDto.City,
                Place = teamDto.Place,
                Type = teamDto.Type,
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

        public async Task<TeamDto> GetById(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Team id required");
                //return;
            }
            var result = await _repository.GetAll()
                .Select(i =>
                new TeamDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Contact = i.Contact,
                    FileName = i.FileName,
                    Zone = i.Zone,
                    IsRegistered = i.IsRegistered,
                    City = i.City,
                    Place = i.Place
                })
                .FirstOrDefaultAsync(i => i.Id == id);
            return result;
        }

        public async Task<ResponseMessageDto> DeleteAsync(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Team id required");
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

        public async Task<List<TeamDto>> GetAll()
        {
            try
            {
               return await _repository.GetAll()
              .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId)
              .Select(i => new TeamDto()
              {
                  Id = i.Id,
                  Name = i.Name,
                  Players = i.TeamPlayers.Where(j => j.TeamId == i.Id).Select(j => j.Player).ToList()

              }).ToListAsync();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams",e);

            }

        }

        public async Task<List<TeamDto>> GetAllTeamsByEventId(long id)
        {
            try
            {
                return await _repository.GetAll()
               .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.EventTeams.Any(j=> j.EventId == id))
               .Select(i => new TeamDto()
               {
                   Id = i.Id,
                   Name = i.Name,
                   Players = i.TeamPlayers.Where(j => j.TeamId == i.Id).Select(j => j.Player).ToList()

               }).ToListAsync();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }

        public async Task<PagedResultDto<TeamDto>> GetPaginatedAllAsync(PagedTeamResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (i.TenantId == _abpSession.TenantId) && (!input.Type.HasValue || i.Type == input.Type))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                    x => x.Name.ToLower().Contains(input.Name.ToLower()));

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderByDescending(i => i.Id)
                .PageBy(input);

            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<TeamDto>(
                totalCount: totalCount,
                items: await pagedAndFilteredPlayers.Select(i => new TeamDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Contact = i.Contact,
                    FileName = i.FileName,
                    Zone = i.Zone,
                    IsRegistered = i.IsRegistered,
                    City = i.City,
                    Place = i.Place,
                    Type = i.Type
                }).ToListAsync());
        }
    }
}

