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
using Abp.UI;
using ScoringAppReact.TeamScores.Dto;
using ScoringAppReact.Players.Dto;
using ScoringAppReact.FallOfWickets.Dto;
using System;

namespace ScoringAppReact.FallOfWickets
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class FallofWicket : AbpServiceBase, IFallofWicketAppService
    {
        private readonly IRepository<FallOfWicket, long> _repository;
        private readonly IAbpSession _abpSession;

        public FallofWicket(IRepository<FallOfWicket, long> repository, IAbpSession abpSession)
        {
            _repository = repository;
            _abpSession = abpSession;
        }

        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateFallofWicketDto model)
        {
            ResponseMessageDto result;
            if (model.Id == 0 || model.Id == null)
            {
                result = await CreateAsync(model);
            }
            else
            {
                result = await UpdateAsync(model);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreateAsync(CreateOrUpdateFallofWicketDto model)
        {
            var alreadyAdded = await _repository.FirstOrDefaultAsync(i => i.MatchId == model.MatchId && i.TeamId == model.TeamId);
            if (alreadyAdded != null)
            {
                throw new UserFriendlyException("Already added with associated team and match");
            }


            var result = await _repository.InsertAsync(new FallOfWicket()
            {
                MatchId = model.MatchId,
                TeamId = model.TeamId,
                First = model.First,
                Second = model.Second,
                Third = model.Third,
                Fourth = model.Fourth,
                Fifth = model.Fifth,
                Sixth = model.Sixth,
                Seventh = model.Seventh,
                Eight = model.Eight,
                Ninth = model.Ninth,
                Tenth = model.Tenth,
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
                    array = new List<Object> { result }
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

        private async Task<ResponseMessageDto> UpdateAsync(CreateOrUpdateFallofWicketDto model)
        {
            var result = await _repository.UpdateAsync(new FallOfWicket()
            {
                Id = model.Id.Value,
                MatchId = model.MatchId,
                TeamId = model.TeamId,
                First = model.First,
                Second = model.Second,
                Third = model.Third,
                Fourth = model.Fourth,
                Fifth = model.Fifth,
                Sixth = model.Sixth,
                Seventh = model.Seventh,
                Eight = model.Eight,
                Ninth = model.Ninth,
                Tenth = model.Tenth,
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
                    array = new List<Object> { result }
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

        public async Task<FallofWicketsDto> GetById(long id)
        {
            var result = await _repository.GetAll()
                .FirstOrDefaultAsync(i => i.Id == id);
            return ObjectMapper.Map<FallofWicketsDto>(result);
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

        public async Task<List<FallofWicketsDto>> GetByTeamIdAndMatchId(long teamId, long matchId)
        {
            var result = await _repository.GetAll().Where(i => i.TeamId == teamId && i.MatchId == matchId && i.TenantId == _abpSession.TenantId).Select(j => new FallofWicketsDto()
            {
                Id = j.Id,
                MatchId = j.MatchId,
                TeamId = j.TeamId,
                First = j.First,
                Second = j.Second,
                Third = j.Third,
                Fourth = j.Fourth,
                Fifth = j.Fifth,
                Sixth = j.Sixth,
                Seventh = j.Seventh,
                Eight = j.Eight,
                Ninth = j.Ninth,
                Tenth = j.Tenth,
            }).FirstOrDefaultAsync();

            return new List<FallofWicketsDto>
            {
                result
            };
        }
        private async Task<PagedResultDto<TeamScoreDto>> GetPaginatedAllAsync(PagedPlayerResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (!input.TenantId.HasValue || i.TenantId == _abpSession.TenantId));

            var pagedAndFilteredPlayers = filteredPlayers.PageBy(input);
            //.OrderBy(i => i.Name)


            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<TeamScoreDto>(
                totalCount: totalCount,
                items: await pagedAndFilteredPlayers.Select(i => new TeamScoreDto()
                {
                    Id = i.Id
                }).ToListAsync());
        }
    }
}

