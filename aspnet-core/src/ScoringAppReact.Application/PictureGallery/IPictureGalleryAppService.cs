using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Http;
using ScoringAppReact.Teams.Dto;
using ScoringAppReact.Teams.InputDto;

namespace ScoringAppReact.PictureGallery
{
    public interface IPictureGalleryAppService : IApplicationService
    {

        Task<ResponseMessageDto> CreateAsync(CreateOrUpdateGalleryDto model);

        Task<ResponseMessageDto> UpdateAsync(CreateOrUpdateGalleryDto model);

        Task<CreateOrUpdateGalleryDto> GetById(long typeId);

        Task<ResponseMessageDto> DeleteAsync(long typeId);

        Task<List<CreateOrUpdateGalleryDto>> GetAll();
        GalleryDto GetImageUrl(PictureDto model);

    }
}
