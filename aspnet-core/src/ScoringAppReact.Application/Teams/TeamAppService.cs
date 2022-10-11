using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ScoringAppReact.Authorization;
using Microsoft.EntityFrameworkCore;
using ScoringAppReact.Models;
using Abp;
using ScoringAppReact.Teams.Dto;
using Abp.Runtime.Session;
using Abp.UI;
using System;
using ScoringAppReact.Matches.Dto;
using ScoringAppReact.Teams.InputDto;
using ScoringAppReact.PictureGallery;
using ScoringAppReact.Teams.Repository;
using ScoringAppReact.Events.Repository;

namespace ScoringAppReact.Teams
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class TeamAppService : AbpServiceBase, ITeamAppService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IRepository<Match, long> _matchRepository;
        private readonly IAbpSession _abpSession;
        private readonly IEventRepository _eventRepository;
        private readonly PictureGalleryAppService _pictureGalleryAppService;
        public TeamAppService(IRepository<Match, long> matchRepository,
            IAbpSession abpSession,
            ITeamRepository teamRepository,
            PictureGalleryAppService pictureGalleryAppService,
            IEventRepository eventRepository
            )
        {
            _teamRepository = teamRepository;
            _abpSession = abpSession;
            _matchRepository = matchRepository;
            _pictureGalleryAppService = pictureGalleryAppService;
            _eventRepository = eventRepository;
        }


        public async Task<ResponseMessageDto> CreateOrEditAsync(CreateOrUpdateTeamDto teamDto)
        {
            ResponseMessageDto result;
            if (teamDto.Id == 0 || teamDto.Id == null)
            {
                result = await CreateTeamAsync(teamDto);
            }
            else
            {
                result = await UpdateTeamAsync(teamDto);
            }
            return result;
        }

        public async Task<TeamDto> GetById(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Team id required");
                //return;
            }
            var result = await _teamRepository.Get(id);

            var mapped = new TeamDto
            {
                Id = result.Id,
                Name = result.Name,
                Contact = result.Contact,
                ProfileUrl = result.ProfileUrl,
                Zone = result.Zone,
                IsRegistered = result.IsRegistered,
                City = result.City,
                Place = result.Place,
                Pictures = result.Pictures.Select(j => new GalleryDto()
                {
                    Id = j.Id,
                    Url = j.Path,
                    Name = j.Name
                }).ToList(),
            };
            return mapped;
        }

        public async Task<ResponseMessageDto> DeleteAsync(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Team id required");
                //return;
            }
            var model = await _teamRepository.Get(id);

            if (model == null)
            {
                throw new UserFriendlyException("No record found with associated Id");
                //return;
            }

            model.IsDeleted = true;
            var result = await _teamRepository.Update(model);

            return new ResponseMessageDto()
            {
                Id = id,
                SuccessMessage = AppConsts.SuccessfullyDeleted,
                Success = true,
                Error = false,
            };
        }

        public async Task<List<TeamListDto>> GetAll()
        {
            try
            {
                var model = await _teamRepository.GetAll(_abpSession.TenantId,null,true);
                return model.Select(i => new TeamListDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    EventId = i.EventTeams.Select(j => j.EventId).FirstOrDefault()

                }).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }

        public async Task<List<TeamDto>> GetAllTeamsByEventId(long id, int? group)
        {
            try
            {
                var model = await _teamRepository.GetAllThenTeamPLayers(_abpSession.TenantId);

                return model.Where(i =>
                i.TenantId == _abpSession.TenantId && i.EventTeams.Any(j => j.EventId == id) &&
               (!group.HasValue || i.EventTeams.Any(j => j.Group == group && j.EventId == id)))
               .Select(i => new TeamDto()
               {
                   Id = i.Id,
                   Name = i.Name,
                   Players = i.TeamPlayers.Where(j => j.TeamId == i.Id).Select(j => j.Player).ToList(),
                   ProfileUrl = i.ProfileUrl

               }).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }

        public async Task<List<GroupWiseTeamsDto>> GetAllTeamsByGroupWiseEventId(long id)
        {
            try
            {
                var result = new List<GroupWiseTeamsDto>();
                var eventData = await _eventRepository.Get(id,_abpSession.TenantId);
                if (eventData == null || eventData.NumberOfGroup == null || eventData.NumberOfGroup < 1)
                {
                    throw new UserFriendlyException("Groups Must be required in League Based Tournament");
                }

                var model = await _teamRepository.GetAll(_abpSession.TenantId,null);

                var allTeams = model
               .Where(i => i.EventTeams.Any(j => j.EventId == id))
               .Select(i => new Dto.EventGroupWiseTeamDto()
               {
                   Id = i.Id,
                   Name = i.Name,
                   Group = i.EventTeams.Where(j => j.EventId == id).Select(j => j.Group).FirstOrDefault()

               }).ToList();

                for (var index = 1; index <= eventData.NumberOfGroup; index++)
                {
                    var GroupWise = new GroupWiseTeamsDto();
                    var team = allTeams.Where(i => i.Group == index).ToList();
                    GroupWise.Teams = team;
                    result.Add(GroupWise);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);

            }

        }

        public async Task<PagedResultDto<TeamDto>> GetPaginatedAllAsync(PagedTeamResultRequestDto input)
        {
            var model = await _teamRepository.GetAll(_abpSession.TenantId, null);
            var filteredTeams = model.Where(i => (!input.Type.HasValue || i.Type == input.Type)
            && (string.IsNullOrWhiteSpace(input.Name) || i.Name.ToLower().Contains(input.Name.ToLower())));

            var pagedAndFilteredTeams = filteredTeams
                .OrderByDescending(i => i.Id);

            var totalCount = filteredTeams.Count();

            return new PagedResultDto<TeamDto>(
                totalCount: totalCount,
                items: pagedAndFilteredTeams.Select(i => new TeamDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    Contact = i.Contact,
                    ProfileUrl = i.ProfileUrl,
                    Zone = i.Zone,
                    IsRegistered = i.IsRegistered,
                    City = i.City,
                    Place = i.Place,
                    Type = i.Type
                }).ToList());
        }

        public async Task<List<TeamDto>> GetAllTeamsByMatchId(long id)
        {
            try
            {
                var match = await _matchRepository.GetAll()
               .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.Id == id)
               .Select(i => new MatchDto()
               {
                   Id = i.Id,
                   Team1 = i.HomeTeam.Name,
                   Team1Id = i.HomeTeam.Id,
                   Team2 = i.OppponentTeam.Name,
                   Team2Id = i.OppponentTeam.Id,
                   Team1ProfileUrl = i.HomeTeam.ProfileUrl,
                   Team2ProfileUrl = i.OppponentTeam.ProfileUrl,

               }).SingleOrDefaultAsync();
                var teamList = new List<TeamDto>();
                for (var x = 0; x < 2; x++)
                {
                    var team = new TeamDto
                    {
                        Id = x == 0 ? match.Team1Id : match.Team2Id,
                        Name = x == 0 ? match.Team1 : match.Team2,
                        ProfileUrl = x == 0 ? match.Team1ProfileUrl : match.Team2ProfileUrl
                    };
                    teamList.Add(team);

                }
                return teamList;

            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }


        public async Task<List<TeamDto>> GetAllTeamsByPlayerId(long id)
        {
            try
            {
                var model = await _teamRepository.GetAll(_abpSession.TenantId, null);
                return model.Where(i => i.TeamPlayers.Any(i => i.PlayerId == id))
                .Select(i => new TeamDto()
                {
                    Id = i.Id,
                    Name = i.Name,
                    ProfileUrl = i.ProfileUrl

                }).ToList();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }


        public async Task<TeamStatsDto> TeamStats(StatsInput input)
        {
            try
            {
                var matches = await _matchRepository.GetAll()
                    .Include(i => i.TeamScores)
                    .Include(i => i.HomeTeam)
                    .Include(i => i.OppponentTeam)
                    .Where(i => (i.HomeTeamId == input.TeamId || i.OppponentTeamId == input.TeamId) &&
                    (!input.MatchTypeId.HasValue || i.MatchTypeId == input.MatchTypeId) &&
                    (!input.Season.HasValue || i.Season == input.Season) &&
                    (!input.EventId.HasValue || i.EventId == input.EventId))
                    .ToListAsync();

                var winningMatch = new List<Match>();
                var loosingMatch = new List<Match>();
                var tieMatch = new List<Match>();
                var noResult = new List<Match>();
                foreach (var item in matches)
                {
                    var t1 = item.TeamScores.Where(i => i.TeamId == input.TeamId).SingleOrDefault();
                    var t2 = item.TeamScores.Where(i => i.TeamId != input.TeamId).SingleOrDefault();
                    if (t1 is null || t2 is null)
                    {
                        noResult.Add(item);
                    }
                    else if (t1.TotalScore > t2.TotalScore)
                    {
                        winningMatch.Add(item);
                    }
                    else if (t1.TotalScore < t2.TotalScore)
                    {
                        loosingMatch.Add(item);
                    }
                    else if (t1.TotalScore == t2.TotalScore)
                    {
                        tieMatch.Add(item);
                    }
                    else
                    {
                        noResult.Add(item);
                    }
                }
                //var teamDetails = matches
                //    .Where(i => i.HomeTeamId == input.TeamId || i.OppponentTeamId == input.TeamId).Select(i => i.HomeTeam).FirstOrDefault();

                var teamDetails = matches
                 .Where(i => i.HomeTeamId == input.TeamId).Select(i => i.HomeTeam).FirstOrDefault() ??
                 matches.Where(i => i.OppponentTeamId == input.TeamId).Select(i => i.OppponentTeam).FirstOrDefault();
                ;
                if (!matches.Any())
                {
                    var team = await _teamRepository.Get(input.TeamId);

                    return new TeamStatsDto
                    {
                        Name = team.Name,
                        Location = team.Place,
                        Type = team.Type,
                        ProfileUrl = team.ProfileUrl
                    };

                }


                var stats = new TeamStatsDto
                {
                    Matches = matches.Count(),
                    Won = winningMatch.Count(),
                    Lost = loosingMatch.Count(),
                    Tie = tieMatch.Count(),
                    NoResult = noResult.Count(),
                    TossWon = matches.Count(i => i.TossWinningTeam == input.TeamId),
                    BatFirst = matches.Count(i => i.HomeTeamId == input.TeamId),
                    FieldFirst = matches.Count(i => i.OppponentTeamId == input.TeamId),
                    Name = teamDetails != null ? teamDetails.Name : "N/A",
                    Location = teamDetails != null ? teamDetails.Place : "N/A",
                    Type = teamDetails != null ? teamDetails.Type : 0,
                    ProfileUrl = teamDetails != null ? teamDetails.ProfileUrl : "Images/dummy.jpg",


                };

                return stats;

            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }

        private async Task<ResponseMessageDto> CreateTeamAsync(CreateOrUpdateTeamDto model)
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

            var result = await _teamRepository.Insert(new Team()
            {
                Name = model.Name,
                Place = model.Place,
                Zone = model.Zone,
                Contact = model.Contact,
                IsRegistered = true,
                City = model.City,
                ProfileUrl = model.ProfileUrl,
                Type = model.Type,
                TenantId = _abpSession.TenantId

            });
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (model.Gallery != null && model.Gallery.Any())
            {
                var gallery = new CreateOrUpdateGalleryDto
                {
                    TeamId = result.Id,
                    Galleries = model.Gallery
                };

                await _pictureGalleryAppService.CreateAsync(gallery);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }


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

        private async Task<ResponseMessageDto> UpdateTeamAsync(CreateOrUpdateTeamDto model)
        {
            if (model.Profile != null)
            {
                if (string.IsNullOrEmpty(model.Profile.Url))
                {

                    var profilePicture = _pictureGalleryAppService.GetImageUrl(model.Profile);
                    model.ProfileUrl = profilePicture.Url;
                }

            }
            var result = await _teamRepository.Update(new Team()
            {
                Id = model.Id.Value,
                Name = model.Name,
                Contact = model.Contact,
                ProfileUrl = model.ProfileUrl,
                Zone = model.Zone,
                IsRegistered = model.IsRegistered,
                City = model.City,
                Place = model.Place,
                Type = model.Type,
                TenantId = _abpSession.TenantId
            });
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (model.Gallery != null)
            {
                var gallery = new CreateOrUpdateGalleryDto
                {
                    TeamId = result.Id,
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



    }
}

