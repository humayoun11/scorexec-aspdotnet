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
using Abp.Runtime.Session;
using Abp.UI;
using ScoringAppReact.Teams.Dto;
using System.Data;
using Abp.EntityFrameworkCore.Repositories;
using Abp.EntityFrameworkCore;
using ScoringAppReact.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using AutoMapper;
using System.Threading;
using System;
using Dapper;
using Abp.Domain.Uow;

namespace ScoringAppReact.Players
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class PlayerAppService : AbpServiceBase, IPlayerAppService
    {
        private readonly IRepository<Player, long> _repository;
        private readonly IRepository<TeamPlayer, long> _teamPlayerRepository;
        private readonly IRepository<PlayerScore, long> _playerScoreRepository;
        private readonly IAbpSession _abpSession;
        private readonly IDbContextProvider<ScoringAppReactDbContext> _context;
        public PlayerAppService(IRepository<Player, long> repository,
            IAbpSession abpSession,
            IRepository<TeamPlayer,
                long> teamPlayerRepository,
            IRepository<PlayerScore,
                long> playerScoreRepository,
            IDbContextProvider<ScoringAppReactDbContext> context)
        {
            _repository = repository;
            _abpSession = abpSession;
            _teamPlayerRepository = teamPlayerRepository;
            _playerScoreRepository = playerScoreRepository;
            _context = context;
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
                ProfileUrl = model.ProfileUrl,
                Gender = model.Gender,
                CreatingTime = model.CreationTime,
                TenantId = _abpSession.TenantId
            });

            await UnitOfWorkManager.Current.SaveChangesAsync();

            var teamPlayerList = new List<TeamPlayer>();
            foreach (var item in model.TeamIds)
            {
                var res = new TeamPlayer()
                {
                    PlayerId = result.Id,
                    TeamId = item
                };
                teamPlayerList.Add(res);
            }

            await _teamPlayerRepository.GetDbContext().AddRangeAsync(teamPlayerList);
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

        public async Task<ResponseMessageDto> CreateOrUpdateTeamPlayersAsync(TeamPlayerDto model)
        {
            var allTeamPlayers = _teamPlayerRepository.GetAll().Where(i => i.TeamId == model.TeamId && i.IsDeleted == false).ToList();
            var prev = allTeamPlayers.Select(i => i.PlayerId);
            var toDelete = prev.Except(model.PlayerIds);
            var toAddNew = model.PlayerIds.Except(prev);
            if (toDelete.Any())
            {
                var deletePlayers = new List<TeamPlayer>();
                foreach (var id in toDelete)
                {
                    var player = allTeamPlayers.Where(j => j.PlayerId == id).FirstOrDefault();
                    player.IsDeleted = true;
                    deletePlayers.Add(player);
                }
                _teamPlayerRepository.GetDbContext().UpdateRange(deletePlayers);
            }
            if (toAddNew.ToList().Any())
            {
                var addNewPlayers = new List<TeamPlayer>();
                foreach (var id in toAddNew)
                {
                    var player = new TeamPlayer()
                    {
                        TeamId = model.TeamId,
                        PlayerId = id
                    };
                    addNewPlayers.Add(player);
                }
                _teamPlayerRepository.GetDbContext().AddRange(addNewPlayers);
            }

            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (model.TeamId != 0)
            {
                return new ResponseMessageDto()
                {
                    Id = model.TeamId,
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

        private async Task<ResponseMessageDto> UpdatePlayerAsync(CreateOrUpdatePlayerDto model)
        {
            var result = await _repository.UpdateAsync(new Player()
            {
                Id = model.Id.Value,
                Name = model.Name,
                Address = model.Address,
                BattingStyleId = model.BattingStyleId,
                BowlingStyleId = model.BowlingStyleId,
                PlayerRoleId = model.PlayerRoleId,
                CNIC = model.CNIC,
                Contact = model.Contact,
                DOB = model.DOB,
                ProfileUrl = model.ProfileUrl,
                Gender = model.Gender,
                CreatingTime = model.CreationTime,
                TenantId = _abpSession.TenantId
            });
            await UnitOfWorkManager.Current.SaveChangesAsync();
            var allTeams = _teamPlayerRepository.GetAll().Where(i => i.PlayerId == result.Id && i.IsDeleted == false).ToList();
            var prev = allTeams.Select(i => i.TeamId);
            var toDelete = prev.Except(model.TeamIds);
            var toAddNew = model.TeamIds.Except(prev);

            if (toDelete.Any())
            {
                var deleteTeams = new List<TeamPlayer>();
                foreach (var id in toDelete)
                {
                    var team = allTeams.Where(j => j.TeamId == id).FirstOrDefault();
                    team.IsDeleted = true;
                    deleteTeams.Add(team);
                }
                _teamPlayerRepository.GetDbContext().UpdateRange(deleteTeams);
            }
            if (toAddNew.ToList().Any())
            {
                var addNewTeams = new List<TeamPlayer>();
                foreach (var id in toAddNew)
                {
                    var team = new TeamPlayer()
                    {
                        TeamId = id,
                        PlayerId = result.Id
                    };
                    addNewTeams.Add(team);
                }
                _teamPlayerRepository.GetDbContext().AddRange(addNewTeams);
            }
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

        public async Task<PlayerEditDto> GetById(long id)
        {
            var result = await _repository.GetAll().Where(i => i.Id == id).Select(i => new PlayerEditDto
            {
                Id = i.Id,
                Name = i.Name,
                Address = i.Address,
                BattingStyleId = i.BattingStyleId,
                BowlingStyleId = i.BowlingStyleId,
                PlayerRoleId = i.PlayerRoleId,
                CNIC = i.CNIC,
                Contact = i.Contact,
                DOB = i.DOB,
                ProfileUrl = i.ProfileUrl,
                Gender = i.Gender,
                TeamIds = i.Teams.Select(i => i.TeamId).ToList()
            }).FirstOrDefaultAsync();
            return result;
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
                ProfileUrl = i.ProfileUrl,
                Contact = i.Contact
            }).ToListAsync();
            return result;
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<PlayerStatisticsDto> GetPlayerStatistics(long playerId, int? matchType, int? season, long? teamId)
        {

            try
            {
                var dbContext = _context.GetDbContext();

                var connection = dbContext.Database.GetDbConnection();
                var paramPlayerId = playerId;
                var paramSeason = matchType;
                var paramMatchTypeId = season;
                var paramTeamId = teamId;
                var result = await connection.QueryFirstOrDefaultAsync<PlayerStatisticsDto>("usp_GetSinglePlayerStatistics",
                    new { paramPlayerId, paramSeason, paramMatchTypeId, paramTeamId },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        public async Task<PlayerStatisticsDto> PlayerStatistics(PlayerStatsInput input)
        {

            try
            {
                var dbContext = _context.GetDbContext();

                var connection = dbContext.Database.GetDbConnection();
                var paramPlayerId = input.PlayerId;
                var paramSeason = input.Season;
                var paramMatchTypeId = input.MatchType;
                var paramTeamId = input.TeamId;
                var result = await connection.QueryFirstOrDefaultAsync<PlayerStatisticsDto>("usp_GetSinglePlayerStatistics",
                    new { paramPlayerId, paramSeason, paramMatchTypeId, paramTeamId },
                    commandType: CommandType.StoredProcedure);
                if (result == null)
                {
                    var stats = new PlayerStatisticsDto();
                    var player = await _repository.GetAll().Where(i => i.Id == input.PlayerId).SingleOrDefaultAsync();
                    if (player == null)
                        return null;
                    stats.PlayerName = player.Name;
                    stats.PlayerRole = player.PlayerRoleId;
                    stats.BattingStyle = player.BattingStyleId;
                    stats.BowlingStyle = player.BowlingStyleId;
                    return stats;
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
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
                    ProfileUrl = i.ProfileUrl
                }).ToListAsync();
            return result;
        }


        public async Task<List<PlayerDto>> AllPlayersByTeamIds(List<long> teamIds)
        {
            var result = await _repository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.Teams.Any(j => teamIds.Contains(j.TeamId)))
                .Select(i => new PlayerDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    ProfileUrl = i.ProfileUrl
                }).ToListAsync();
            return result;
        }


        public async Task<List<PlayerDto>> GetAllByMatchId(long id)
        {
            var result = await _playerScoreRepository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.MatchId == id)
                .Select(i => new PlayerDto()
                {
                    Id = i.Id,
                    Name = i.Player.Name,
                }).ToListAsync();
            return result;
        }

        public async Task<List<PlayerDto>> GetAllByEventId(long id)
        {
            var result = await _playerScoreRepository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.Match.EventId == id)
                .Select(i => new PlayerDto()
                {
                    Id = i.Id,
                    Name = i.Player.Name,
                }).ToListAsync();
            return result;
        }

        public async Task<List<PlayerListDto>> GetTeamPlayersByMatchId(long matchId)
        {
            var result = await _playerScoreRepository.GetAll()
                .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.MatchId == matchId)
                .Select(i => new PlayerListDto()
                {
                    Id = i.Id,
                    Name = i.Player.Name,
                    TeamId = i.TeamId
                }).ToListAsync();
            return result;
        }

        public async Task<PagedResultDto<PlayerDto>> GetPaginatedAllAsync(PagedPlayerResultRequestDto input)
        {
            var filteredPlayers = _repository.GetAll()
                .Where(i => i.IsDeleted == false && (i.TenantId == _abpSession.TenantId))
                .WhereIf(input.TeamId.HasValue, i => i.Teams.Any(j => j.TeamId == input.TeamId))
                .WhereIf(input.PlayingRole.HasValue, i => i.PlayerRoleId == input.PlayingRole)
                .WhereIf(input.BattingStyle.HasValue, i => i.BattingStyleId == input.BattingStyle)
                .WhereIf(input.BowlingStyle.HasValue, i => i.BowlingStyleId == input.BowlingStyle)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Name),
                    x => x.Name.ToLower().Contains(input.Name.ToLower()))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Contact),
                    x => x.Contact.ToLower().Contains(input.Contact.ToLower()));

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderByDescending(i => i.Id)
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
                    ProfileUrl = i.ProfileUrl,
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

