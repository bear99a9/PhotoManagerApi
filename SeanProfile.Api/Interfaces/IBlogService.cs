
namespace SeanProfile.Api.Services
{
    public interface IBlogService
    {
        Task<ServiceResponse<IList<BlogModel>>> SaveFile(IEnumerable<IFormFile> files);
    }
}