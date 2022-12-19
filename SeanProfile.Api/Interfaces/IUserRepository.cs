
using SeanProfile.Api.Model;

namespace SeanProfile.Api.DataLayer
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string email);
        Task<int> InsertNewUser(UserModel user);
        Task<UserModel> GetUserByEmail<T>(T user);
        Task<UserModel> GetUserById<T>(T user);
        Task UpdateUserPassword(UserModel user);
        Task<IEnumerable<UserModel>> GetAllUsersToEmail();
        Task InsertPasswordReset(PasswordReset passwordReset);
        Task<PasswordReset> RetrievePasswordReset(string passwordResetKey);
        Task UpdatePasswordReset(string passwordResetKey);
    }
}