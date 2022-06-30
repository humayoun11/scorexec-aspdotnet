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
using Abp.UI;
using System;
using ScoringAppReact.Matches.Dto;
using ScoringAppReact.Teams.InputDto;
using ScoringAppReact.Events;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Abp.Domain.Uow;
using ScoringAppReact.PictureGallery;

namespace ScoringAppReact.Teams
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class TeamAppService : AbpServiceBase, ITeamAppService
    {
        private readonly IRepository<Team, long> _repository;
        private readonly IRepository<Match, long> _matchRepository;
        private readonly IAbpSession _abpSession;
        private readonly EventAppService _eventAppService;
        private readonly IWebHostEnvironment _hosting;
        private readonly PictureGalleryAppService _pictureGalleryAppService;
        public TeamAppService(IRepository<Team, long> repository, IRepository<Match, long> matchRepository,
            IAbpSession abpSession,
            EventAppService eventAppService,
            IWebHostEnvironment hosting,
            PictureGalleryAppService pictureGalleryAppService
            )
        {
            _repository = repository;
            _abpSession = abpSession;
            _eventAppService = eventAppService;
            _matchRepository = matchRepository;
            _hosting = hosting;
            _pictureGalleryAppService = pictureGalleryAppService;
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

            var result = await _repository.InsertAsync(new Team()
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

            if (model.Gallery.Any())
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
            var result = await _repository.UpdateAsync(new Team()
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

            if (model.Gallery.Any())
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
                    ProfileUrl = i.ProfileUrl,
                    Zone = i.Zone,
                    IsRegistered = i.IsRegistered,
                    City = i.City,
                    Place = i.Place,
                    Pictures = i.Pictures.Select(i => new GalleryDto()
                    {
                        Id = i.Id,
                        Url = i.Path,
                        Name = i.Name
                    }).ToList(),
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

        public async Task<List<TeamListDto>> GetAll()
        {
            try
            {
                return await _repository.GetAll()
               .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId)
               .Select(i => new TeamListDto()
               {
                   Id = i.Id,
                   Name = i.Name,
                   EventId = i.EventTeams.Select(j => j.EventId).FirstOrDefault()

               }).ToListAsync();
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
                return await _repository.GetAll()
               .Where(i => i.IsDeleted == false &&
                i.TenantId == _abpSession.TenantId && i.EventTeams.Any(j => j.EventId == id))
               .WhereIf(group.HasValue, i => i.EventTeams.Any(j => j.Group == group && j.EventId == id))
               .Select(i => new TeamDto()
               {
                   Id = i.Id,
                   Name = i.Name,
                   Players = i.TeamPlayers.Where(j => j.TeamId == i.Id).Select(j => j.Player).ToList(),
                   ProfileUrl = i.ProfileUrl

               }).ToListAsync();
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
                var eventData = await _eventAppService.GetById(id);
                if (eventData == null || eventData.NumberOfGroup == null || eventData.NumberOfGroup < 1)
                {
                    throw new UserFriendlyException("Groups Must be required in League Based Tournament");
                }

                var allTeams = await _repository.GetAll()
               .Where(i => i.IsDeleted == false &&
                i.TenantId == _abpSession.TenantId && i.EventTeams.Any(j => j.EventId == id))
               .Select(i => new Dto.EventGroupWiseTeamDto()
               {
                   Id = i.Id,
                   Name = i.Name,
                   Group = i.EventTeams.Where(j => j.EventId == id).Select(j => j.Group).FirstOrDefault()

               }).ToListAsync();

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
                    ProfileUrl = i.ProfileUrl,
                    Zone = i.Zone,
                    IsRegistered = i.IsRegistered,
                    City = i.City,
                    Place = i.Place,
                    Type = i.Type
                }).ToListAsync());
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

               }).SingleOrDefaultAsync();
                var teamList = new List<TeamDto>();
                for (var x = 0; x < 2; x++)
                {
                    var team = new TeamDto
                    {
                        Id = x == 0 ? match.Team1Id : match.Team2Id,
                        Name = x == 0 ? match.Team1 : match.Team2,
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
                return await _repository.GetAll()
               .Where(i => i.IsDeleted == false && i.TenantId == _abpSession.TenantId && i.TeamPlayers.Any(i => i.PlayerId == id))
               .Select(i => new TeamDto()
               {
                   Id = i.Id,
                   Name = i.Name,
                   ProfileUrl = i.ProfileUrl

               }).ToListAsync();
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
                    var team = await _repository.GetAll().Where(i => i.IsDeleted == false && i.Id == input.TeamId).SingleOrDefaultAsync();

                    return new TeamStatsDto
                    {
                        Name = team.Name,
                        Location = team.Place,
                        Type = team.Type
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
                    ProfileUrl = teamDetails != null ? teamDetails.ProfileUrl : "dummy.jpg",


                };

                return stats;

            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }


        private string SaveImagesBase64Async(PictureDto sender)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(sender.Name).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssff") + Path.GetExtension(sender.Name);
            var path = Path.Combine(_hosting.ContentRootPath, "Images", imageName);
            var b = sender.Blob.Split("base64,")[1];
            File.WriteAllBytes(path, Convert.FromBase64String(b));
            return imageName;
        }


    }
}

