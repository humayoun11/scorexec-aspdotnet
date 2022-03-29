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
using Microsoft.AspNetCore.Mvc;
using ScoringAppReact.Grounds.Dto;
using Abp.UI;

namespace ScoringAppReact.Grounds
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class GroundAppService : AbpServiceBase, IGroundAppService
    {
        private readonly IRepository<Ground, long> _repository;
        private readonly IAbpSession _abpSession;
        public GroundAppService(IRepository<Ground, long> repository, IAbpSession abpSession)
        {
            _repository = repository;
            _abpSession = abpSession;
        }


        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateGroundDto model)
        {
            ResponseMessageDto result;
            if (model.Id == 0)
            {
                result = await CreateGroundAsync(model);
            }
            else
            {
                result = await UpdateGroundAsync(model);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreateGroundAsync(CreateOrUpdateGroundDto model)
        {


            var result = await _repository.InsertAsync(new Ground()
            {
                Name = model.Name,
                Location = model.Location,
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

        private async Task<ResponseMessageDto> UpdateGroundAsync(CreateOrUpdateGroundDto model)
        {
            var result = await _repository.UpdateAsync(new Ground()
            {
                Name = model.Name,
                Location = model.Location,
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

        public async Task<GroundDto> GetById(long id)
        {
            if (id == 0 )
            {
                throw new UserFriendlyException("Id cannot be zero");
                //return;
            }

            var result = await _repository.GetAll()
                .Select(i =>
                new GroundDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Location = i.Location
                })
                .FirstOrDefaultAsync(i => i.Id == id);
            return result;
        }

        public async Task<ResponseMessageDto> DeleteAsync(long id)
        {
            if(id == 0)
            {
                throw new UserFriendlyException("Id cannot be zero");
            }
            var model = await _repository.FirstOrDefaultAsync(i => i.Id == id);
            if(model == null)
            {
                throw new UserFriendlyException("Record not found");
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

        public async Task<List<GroundDto>> GetAll()
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId)
                .Select(i => new GroundDto()
                {
                    Id = i.Id,
                    Name = i.Name
                }).ToListAsync();
            return result;
        }

        public async Task<PagedResultDto<GroundDto>> GetPaginatedAllAsync(PagedGroundResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (!input.TenantId.HasValue || i.TenantId == input.TenantId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                    x => x.Name.Contains(input.Name));

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderBy(i => i.Name)
                .PageBy(input);

            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<GroundDto>(
                totalCount: totalCount,
                items: await pagedAndFilteredPlayers.Select(i => new GroundDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Location = i.Location
                }).ToListAsync());
        }
    }
}

