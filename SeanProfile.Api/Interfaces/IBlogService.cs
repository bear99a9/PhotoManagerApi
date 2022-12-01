
namespace SeanProfile.Api.Services
{
    public interface IBlogService
    {
        Task<ServiceResponse<IList<string>>> SaveFile(IEnumerable<IFormFile> files);
    }
}