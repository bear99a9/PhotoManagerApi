
using SeanProfile.Api.Model;

namespace SeanProfile.Api.DataLayer
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string email);
        Task InsertNewUser(UserModel user);
        Task<UserModel> GetUserByEmail(UserModel user);
    }
}