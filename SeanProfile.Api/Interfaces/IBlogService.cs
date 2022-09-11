
namespace SeanProfile.Api.Services
{
    public interface IBlogService
    {
        Task<ServiceResponse<IList<UploadResult>>> SaveFile(IEnumerable<IFormFile> files);
    }
}