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
using ScoringAppReact.PictureGallery;
using ScoringAppReact.EntityAdmins.Dto;
using ScoringAppReact.EntityAdmins;
using ScoringAppReact.Players.Repository;
using ScoringAppReact.PlayerScores.Repository;

namespace ScoringAppReact.Players
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class PlayerAppService : AbpServiceBase, IPlayerAppService
    {
        private readonly IRepository<TeamPlayer, long> _teamPlayerRepository;
        private readonly IPlayerScoreRepository _playerScoreRepository;
        private readonly IAbpSession _abpSession;
        private readonly IDbContextProvider<ScoringAppReactDbContext> _context;
        private readonly PictureGalleryAppService _pictureGalleryAppService;
        private readonly EntityAdminAppService _entityAppService;
        private readonly IPlayerRepository _playerRepository;
        public PlayerAppService(
            IAbpSession abpSession,
            IRepository<TeamPlayer,
                long> teamPlayerRepository,
            IPlayerScoreRepository playerScoreRepository,
            IDbContextProvider<ScoringAppReactDbContext> context,
            PictureGalleryAppService pictureGalleryAppService,
            EntityAdminAppService entityAppService,
            IPlayerRepository playerRepository
            )
        {
            _abpSession = abpSession;
            _teamPlayerRepository = teamPlayerRepository;
            _playerScoreRepository = playerScoreRepository;
            _context = context;
            _pictureGalleryAppService = pictureGalleryAppService;
            _entityAppService = entityAppService;
            _playerRepository = playerRepository;
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
            if (model.Profile != null)
            {
                if (string.IsNullOrEmpty(model.Profile.Url))
                {

                    var profilePicture = _pictureGalleryAppService.GetImageUrl(model.Profile);
                    model.ProfileUrl = profilePicture.Url;
                }
            }

            if (await CheckPhoneNumber(model.Contact, null))
            {
                throw new UserFriendlyException("This Phone number is already assosicated with another account");
            }


            var result = await _playerRepository.Insert(new Player()
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
            if (model.TeamIds != null)
            {
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

            }



            if (model.Gallery != null && model.Gallery.Any())
            {
                var gallery = new CreateOrUpdateGalleryDto
                {
                    PlayerId = result.Id,
                    Galleries = model.Gallery
                };

                await _pictureGalleryAppService.CreateAsync(gallery);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            long[] userIds = new long[1] { _abpSession.UserId.Value };

            var entityAdmin = new CreateOrEditEntityAdmin { PlayerId = result.Id, UserIds = userIds };

            await _entityAppService.CreateOrEditAsync(entityAdmin);

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
            if (model.Profile != null)
            {
                if (string.IsNullOrEmpty(model.Profile.Url))
                {

                    var profilePicture = _pictureGalleryAppService.GetImageUrl(model.Profile);
                    model.ProfileUrl = profilePicture.Url;
                }

            }

            if (await CheckPhoneNumber(model.Contact, model.Id))
            {
                throw new UserFriendlyException("This Phone number is already assosicated with another account");
            }

            var result = await _playerRepository.Update(new Player()
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

            if (model.Gallery != null)
            {
                var gallery = new CreateOrUpdateGalleryDto
                {
                    PlayerId = result.Id,
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

        public async Task<PlayerEditDto> GetById(long id)
        {
            var result = await _playerRepository.Get(id);
            var player = new PlayerEditDto
            {
                Id = result.Id,
                Name = result.Name,
                Address = result.Address,
                BattingStyleId = result.BattingStyleId,
                BowlingStyleId = result.BowlingStyleId,
                PlayerRoleId = result.PlayerRoleId,
                CNIC = result.CNIC,
                Contact = result.Contact,
                DOB = result.DOB,
                ProfileUrl = result.ProfileUrl,
                Gender = result.Gender,
                TeamIds = result.Teams.Select(i => i.TeamId).ToList(),
                Pictures = result.Pictures?.Select(j => new GalleryDto()
                {
                    Id = j.Id,
                    Url = j.Path,
                    Name = j.Name
                }).ToList(),
            };
            return player;

        }

        public async Task<ResponseMessageDto> DeleteAsync(long playerId)
        {
            var model = await _playerRepository.Get(playerId);
            model.IsDeleted = true;
            await _playerRepository.Update(model);

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
            var result = await _playerRepository.GetAll(tenantId: _abpSession.TenantId, null);
            return result.Select(i => new PlayerDto()
            {
                Id = i.Id,
                Name = i.Name,
                ProfileUrl = i.ProfileUrl,
                Contact = i.Contact
            }).ToList();
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
                    var player = await _playerRepository.Get(input.PlayerId);
                    if (player == null)
                        return null;
                    stats.Name = player.Name;
                    stats.PlayerRole = player.PlayerRoleId;
                    stats.BattingStyle = player.BattingStyleId;
                    stats.BowlingStyle = player.BowlingStyleId;
                    stats.ProfileUrl = player.ProfileUrl;
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
            var result = await _playerRepository.GetAll(tenantId: _abpSession.TenantId, teamId: teamId);

            return result
            .Select(i => new PlayerDto()
            {
                Id = i.Id,
                Name = i.Name,
                ProfileUrl = i.ProfileUrl
            }).ToList();
        }


        public async Task<List<PlayerDto>> AllPlayersByTeamIds(List<long> teamIds)
        {
            var result = await _playerRepository.GetAllByTeamIds(teamIds, _abpSession.TenantId);
            return result.Select(i => new PlayerDto()
            {
                Id = i.Id,
                Name = i.Name,
                ProfileUrl = i.ProfileUrl
            }).ToList();
        }


        public async Task<List<PlayerDto>> GetAllByMatchId(long id)
        {
            var result = await _playerScoreRepository.GetAllPlayers(matchId: id, null, tenantId: _abpSession.TenantId);
            return result.Select(i => new PlayerDto()
            {
                Id = i.Id,
                Name = i.Player.Name,
                ProfileUrl = i.Player.ProfileUrl
            }).ToList();
        }

        public async Task<List<PlayerDto>> GetAllByEventId(long id)
        {
            var result = await _playerScoreRepository.GetAllPlayers(null, id, _abpSession.TenantId);
            return result.Select(i => new PlayerDto()
            {
                Id = i.Id,
                Name = i.Player.Name,
                ProfileUrl = i.Player.ProfileUrl
            }).ToList();
        }

        public async Task<List<PlayerListDto>> GetTeamPlayersByMatchId(long matchId, long? teamId)
        {
            var result = await _playerScoreRepository.GetAll(teamId, matchId, null, null, _abpSession.TenantId, true, false);
            return result.Select(i => new PlayerListDto()
            {
                Id = i.Id,
                Name = i.Player.Name,
                TeamId = i.TeamId,
                ProfileUrl = i.Player.ProfileUrl,

            }).ToList();
        }

        public async Task<PagedResultDto<PlayerDto>> GetPaginatedAllAsync(PagedPlayerResultRequestDto input)
        {
            var filteredPlayers = await _playerRepository.GetAllPaginated(input, _abpSession.TenantId);

            var pagedAndFilteredPlayers = filteredPlayers
                .OrderByDescending(i => i.Id);

            var totalCount = filteredPlayers.Count();

            return new PagedResultDto<PlayerDto>(
                totalCount: totalCount,
                items: pagedAndFilteredPlayers.Select(i => new PlayerDto()
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
                    Address = i.Address
                }).ToList()); ;
        }


        private async Task<bool> CheckPhoneNumber(string phone, long? id)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new UserFriendlyException("Phone Number is required");
            }
            var mode = await _playerRepository.GetAll(_abpSession.TenantId, id);
            return mode.Any(i => i.Contact == phone);
        }
    }
}

