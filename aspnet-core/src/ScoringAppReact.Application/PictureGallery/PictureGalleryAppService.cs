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
using Abp.EntityFrameworkCore.Repositories;

namespace ScoringAppReact.PictureGallery
{
    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class PictureGalleryAppService : AbpServiceBase, IPictureGalleryAppService
    {
        private readonly IRepository<Gallery, long> _repository;
        private readonly IAbpSession _abpSession;
        private readonly IWebHostEnvironment _hosting;
        public PictureGalleryAppService(
            IRepository<Gallery, long> repository,
            IAbpSession abpSession,
            IWebHostEnvironment hosting
            )
        {
            _repository = repository;
            _abpSession = abpSession;
            _hosting = hosting;
        }


        public async Task<ResponseMessageDto> CreateAsync(CreateOrUpdateGalleryDto model)
        {

            var entityId = getEnitityId(model);
            if (entityId == 0)
                throw new UserFriendlyException("EntityId can not be null or 0");

            //var newIds = model.Galleries.Select(i => i.Id).ToList();

            //var oldGalleriesIds = await _repository.GetAll()
            //    .Where(i => i.MatchId == entityId && i.IsDeleted == false).Select(i => i.Id)
            //    .ToListAsync();

            //var toAddTeams = selectedTeam.Except(oldGalleriesIds).ToList();
            //var ToDelete = oldGalleriesIds.Except(selectedTeam).ToList();

            var gallery = new List<Gallery>();
            foreach (var item in model.Galleries)
            {
                //var data = new Gallery();
                //data.Path = item.Url;
                //data.Name = item.Name;

                var data = SaveImagesBase64Async(item);
                var result = new Gallery()
                {
                    Path = data.Url,
                    Name = data.Name,
                    MatchId = model.MatchId ?? null,
                    EventId = model.EventId ?? null,
                    PlayerId = model.PlayerId ?? null,
                    GroundId = model.GroundId ?? null,
                    TeamId = model.TeamId ?? null,
                };
                gallery.Add(result);

            }
            await _repository.GetDbContext().AddRangeAsync(gallery);
            await UnitOfWorkManager.Current.SaveChangesAsync();

            if (gallery[0].Id != 0)
            {
                return new ResponseMessageDto()
                {
                    Id = entityId,
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


        public async Task<ResponseMessageDto> UpdateAsync(CreateOrUpdateGalleryDto model)
        {

            var entityId = getEnitityId(model);
            if (entityId == 0)
                throw new UserFriendlyException("EntityId can not be null or 0");

            var newIds = model.Galleries.Where(i => i.Id.HasValue).Select(i => i.Id.Value).ToList();

            var galleries = await _repository.GetAll()
             .Where(i => (!i.MatchId.HasValue || i.MatchId == entityId) &&
                (!i.TeamId.HasValue || i.TeamId == entityId) &&
                 (!i.PlayerId.HasValue || i.PlayerId == entityId) &&
                  (!i.GroundId.HasValue || i.GroundId == entityId) &&
                  (!i.EventId.HasValue || i.EventId == entityId) &&
                  i.IsDeleted == false)
             .ToListAsync();

            var oldIds = galleries.Select(i => i.Id).ToList();

            var toAddTeams = model.Galleries.Where(i => !i.Id.HasValue).ToList();

            var ToDelete = oldIds.Except(newIds).ToList();

            if (ToDelete.Any())
            {
                var toDeleteGallery = new List<Gallery>();
                foreach (var id in ToDelete)
                {
                    var result = galleries.Where(i => i.Id == id).SingleOrDefault();
                    result.IsDeleted = true;
                    toDeleteGallery.Add(result);
                }
                _repository.GetDbContext().UpdateRange(toDeleteGallery);
                await UnitOfWorkManager.Current.SaveChangesAsync();

            }

            var gallery = new List<Gallery>();
            if (toAddTeams.Any())
            {
                foreach (var item in toAddTeams)
                {
                    var data = SaveImagesBase64Async(item);
                    var result = new Gallery()
                    {
                        Path = data.Url,
                        Name = data.Name,
                        MatchId = model.MatchId ?? null,
                        EventId = model.EventId ?? null,
                        PlayerId = model.PlayerId ?? null,
                        GroundId = model.GroundId ?? null,
                        TeamId = model.TeamId ?? null,
                    };
                    gallery.Add(result);

                }
                await _repository.GetDbContext().AddRangeAsync(gallery);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }


            if (entityId != 0)
            {
                return new ResponseMessageDto()
                {
                    Id = entityId,
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

        public async Task<CreateOrUpdateGalleryDto> GetById(long id)
        {
            if (id == 0)
            {
                throw new UserFriendlyException("Team id required");
                //return;
            }
            var result = await _repository.GetAll()
                .Select(i =>
                new CreateOrUpdateGalleryDto()
                {
                    Id = i.Id,
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

        public async Task<List<CreateOrUpdateGalleryDto>> GetAll()
        {
            try
            {
                return await _repository.GetAll()
               .Where(i => i.IsDeleted == false)
               .Select(i => new CreateOrUpdateGalleryDto()
               {
                   Id = i.Id

               }).ToListAsync();
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("Something went wrong with geeting all teams", e);

            }

        }

        public GalleryDto GetImageUrl(PictureDto model)
        {
            return SaveImagesBase64Async(model);
        }

        [AbpAllowAnonymous]
        [UnitOfWork(isTransactional: false)]
        private GalleryDto SaveImagesBase64Async(PictureDto sender)
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(sender.Name).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssff") + Path.GetExtension(sender.Name);
            var path = Path.Combine(_hosting.ContentRootPath, "wwwroot", "Images", imageName);
            var b = sender.Blob.Split("base64,")[1];
            File.WriteAllBytes(path, Convert.FromBase64String(b));
            var data = new GalleryDto()
            {
                Url = $"Images/{imageName}",
                Name = imageName
            };
            return data;
        }


        private long getEnitityId(CreateOrUpdateGalleryDto model)
        {

            if (model.MatchId.HasValue)
                return model.MatchId.Value;
            if (model.TeamId.HasValue)
                return model.TeamId.Value;
            if (model.EventId.HasValue)
                return model.EventId.Value;
            if (model.PlayerId.HasValue)
                return model.PlayerId.Value;
            if (model.GroundId.HasValue)
                return model.GroundId.Value;

            return 0;

        }



    }
}

