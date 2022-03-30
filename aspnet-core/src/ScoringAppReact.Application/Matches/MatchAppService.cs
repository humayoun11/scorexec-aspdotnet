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
        private readonly IAbpSession _abpSession;
        public MatchAppService(IRepository<Match, long> repository, IAbpSession abpSession)
        {
            _repository = repository;
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
                    HomeTeamId = model.Team1_Id,
                    OppponentTeamId = model.Team2_Id,
                    MatchDescription = model.MatchDescription,
                    DateOfMatch = model.DateOfMatch,
                    Season = model.Season,
                    MatchTypeId = model.MatchTypeId,
                    TossWinningTeam = model.TossWinningTeam,
                    PlayerOTM = model.PlayerOTM,
                    EventId = model.EventId,
                    EventStage = model.EventStage,
                    FileName = model.FileName,
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
                HomeTeamId = model.Team1_Id,
                OppponentTeamId = model.Team2_Id,
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

        public async Task<MatchDto> GetById(long matchId)
        {
            var result = await _repository
                .FirstOrDefaultAsync(i => i.Id == matchId);
            if (result == null)
                throw new UserFriendlyException("No Record Exists");
            return ObjectMapper.Map<MatchDto>(result);
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

        public async Task<List<MatchDto>> GetAll(long? tenantId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId)
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

        public async Task<PagedResultDto<MatchDto>> GetPaginatedAllAsync(PagedMatchResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (!input.TenantId.HasValue || i.TenantId == input.TenantId));
            //.WhereIf(!string.IsNullOrWhiteSpace(input.Name),
            //    x => x.Name.Contains(input.Name));

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderBy(i => i.DateOfMatch)
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
                }).ToListAsync());
        }
    }
}

