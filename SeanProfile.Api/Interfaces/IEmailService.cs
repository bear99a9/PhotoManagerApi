
namespace SeanProfile.Api.Services
{
    public interface IEmailService
    {
        Task SendNewUploadEmail(IEnumerable<string> emails);
    }
}