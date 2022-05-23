using SeanProfile.Api.Model;

namespace SeanProfile.Api.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserModel user);
    }
}