using SeanProfile.Api.Model;

namespace SeanProfile.Api.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<int>> Register(UserModel user);
        Task<ServiceResponse<string>> Login(UserLogin userLogin);
        Task<ServiceResponse<bool>> ChangePassword(UserChangePassword changePassword);    
    }
}