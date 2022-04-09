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
using ScoringAppReact.Players.Dto;
using System;
using Abp.Runtime.Session;
using Abp.UI;
using ScoringAppReact.Teams.Dto;
using System.Data;

namespace ScoringAppReact.Players
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class PlayerAppService : AbpServiceBase, IPlayerAppService
    {
        private readonly DbContext _context;
        private readonly IRepository<Player, long> _repository;
        private readonly IRepository<TeamPlayer, long> _teamPlayerRepository;
        private readonly IAbpSession _abpSession;
        public PlayerAppService(IRepository<Player, long> repository,
            IAbpSession abpSession,
            IRepository<TeamPlayer,
                long> teamPlayerRepository, DbContext context)
        {
            _context = context;
            _repository = repository;
            _abpSession = abpSession;
            _teamPlayerRepository = teamPlayerRepository;
        }

        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdatePlayerDto model)
        {
            ResponseMessageDto result;
            if (model.Id == 0 || model.Id == null)
            {
                result = await CreatePlayerAsync(model);
            }
            else
            {
                result = await UpdatePlayerAsync(model);
            }
            return result;
        }

        private async Task<ResponseMessageDto> CreatePlayerAsync(CreateOrUpdatePlayerDto model)
        {
            if (string.IsNullOrEmpty(model.Name))
            {
                throw new UserFriendlyException("Name must required");
                //return;
            }


            var result = await _repository.InsertAsync(new Player()
            {
                Name = model.Name,
                Address = model.Address,
                BattingStyleId = model.BattingStyleId,
                BowlingStyleId = model.BowlingStyleId,
                PlayerRoleId = model.PlayerRoleId,
                CNIC = model.CNIC,
                Contact = model.Contact,
                DOB = model.DOB,
                FileName = model.FileName,
                Gender = model.Gender,
                CreatingTime = model.CreationTime,
                TenantId = _abpSession.TenantId
            });

            await UnitOfWorkManager.Current.SaveChangesAsync();

            var teamPlayer = await _teamPlayerRepository.InsertAsync(new TeamPlayer()
            {
                TeamId = model.TeamId,
                PlayerId = result.Id
            });

            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (result.Id != 0 && teamPlayer.Id != 0)
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

        private async Task<ResponseMessageDto> UpdatePlayerAsync(CreateOrUpdatePlayerDto model)
        {
            var result = await _repository.UpdateAsync(new Player()
            {
                Name = model.Name,
                Address = model.Address,
                BattingStyleId = model.BattingStyleId,
                BowlingStyleId = model.BowlingStyleId,
                PlayerRoleId = model.PlayerRoleId,
                CNIC = model.CNIC,
                Contact = model.Contact,
                DOB = model.DOB,
                FileName = model.FileName,
                Gender = model.Gender,
                CreatingTime = model.CreationTime,
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

        public async Task<PlayerDto> GetById(long id)
        {
            var result = await _repository
                .FirstOrDefaultAsync(i => i.Id == id);
            return ObjectMapper.Map<PlayerDto>(result);
        }

        public async Task<ResponseMessageDto> DeleteAsync(long playerId)
        {
            var model = await _repository.FirstOrDefaultAsync(i => i.Id == playerId);
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

        public async Task<List<PlayerDto>> GetAll()
        {
            var result = await _repository.GetAll().Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId).Select(i => new PlayerDto()
            {
                Id = i.Id,
                Name = i.Name,
                FileName = i.FileName
            }).ToListAsync();
            return result;
        }


        public async Task<PlayerStatisticsDto> PlayerStatistics(int id)
        {

            //ViewBag.Season = new SelectList(_context.Matches.Select(i => i.Season).ToList().Distinct(), "Season");

            // ViewBag.MatchType = new SelectList(_context.MatchType, "MatchTypeId", "MatchTypeName");
            //ViewBag.Overs = new SelectList(_context.Matches.Select(i => i.MatchOvers).ToList().Distinct(), "MatchOvers");
            try
            {
                //var connection = _context.Database.GetDbConnection();
                //var model = connection.QuerySingleOrDefault<PlayerStatisticsDto>(
                //    "usp_GetSinglePlayerStatistics",
                //    new
                //    {
                //        @paramPlayerId = id

                //    },
                //    commandType: CommandType.StoredProcedure) ?? new PlayerStatisticsDto
                //    {

                //    };
                //return model;
                return null;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task<List<PlayerDto>> GetAllByTeamId(long teamId)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.Teams.Any(j => j.TeamId == teamId))
                .Select(i => new PlayerDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    FileName = i.FileName
                }).ToListAsync();
            return result;
        }

        public async Task<PagedResultDto<PlayerDto>> GetPaginatedAllAsync(PagedPlayerResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (!input.TenantId.HasValue || i.TenantId == input.TenantId))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                    x => x.Name.Contains(input.Name));

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderBy(i => i.Name)
                .PageBy(input);

            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<PlayerDto>(
                totalCount: totalCount,
                items: await pagedAndFilteredPlayers.Select(i => new PlayerDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    BattingStyleId = i.BattingStyleId,
                    BowlingStyleId = i.BowlingStyleId,
                    PlayerRoleId = i.PlayerRoleId,
                    DOB = i.DOB,
                    IsDeactivated = i.IsDeactivated,
                    Contact = i.Contact,
                    FileName = i.FileName,
                    Address = i.Address,
                    Teams = i.Teams.Where(j => j.PlayerId == i.Id).Select(j => j.Team).Select(k => new TeamDto()
                    {
                        Id = k.Id,
                        Name = k.Name,
                        Type = k.Type

                    }).ToList()


                }).ToListAsync()); ;
        }
    }
}

