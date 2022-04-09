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

namespace ScoringAppReact.TeamScores
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class TeamScoresAppService : AbpServiceBase, ITeamScoresAppService
    {
        private readonly IRepository<TeamScore, long> _repository;
        private readonly IAbpSession _abpSession;

        public TeamScoresAppService(IRepository<TeamScore, long> repository, IAbpSession abpSession)
        {
            _repository = repository;
            _abpSession = abpSession;
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
            //if (string.IsNullOrEmpty(model.Name))
            //{
            //    Console.WriteLine("PLayer Name Missing");
            //    //return;
            //}


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

