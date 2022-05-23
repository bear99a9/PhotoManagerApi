using SeanProfile.Api.Model;

namespace SeanProfile.Api.Services
{
    public interface IAuthService
    {
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        string CreateToken(UserModel user);
        RefreshToken GenerateRefreshToken();
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}