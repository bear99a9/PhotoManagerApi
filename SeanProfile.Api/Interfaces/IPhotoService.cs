﻿
namespace SeanProfile.Api.Services
{
    public interface IPhotoService
    {
        Task<ServiceResponseModel<IList<string>>> SavePhoto(IEnumerable<IFormFile> files, int userId);
    }
}