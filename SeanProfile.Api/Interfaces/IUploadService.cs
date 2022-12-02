
namespace SeanProfile.Api.Services
{
    public interface IUploadService
    {
        Task<ServiceResponseModel<IList<string>>> SaveFile(IEnumerable<IFormFile> files, int userId);
    }
}