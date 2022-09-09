using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SeanProfile.Api.DataLayer;
using SeanProfile.Api.Model;
using SeanProfile.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SeanProfile.Api.Services
{
    public class AuthService : IAuthService
    {
        public readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepo;

        public AuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepo = userRepository;
        }


        public async Task<ServiceResponse<int>> Register(UserModel user)
        {
            if (await UserExists(user.Email))
            {
                return new ServiceResponse<int> { Success = false, Message = "Email address already exists" };
            }

            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Role = "Guest";
            user.Email = user.Email.ToLower();
            
            await _userRepo.InsertNewUser(user);

            user = await _userRepo.GetUserByEmail(user);

            return new ServiceResponse<int> { Data = user.Id, Message = "User added successfully" };
        }

        public async Task<ServiceResponse<string>> Login(UserLogin userLogin)
        {
           
            userLogin.Email = userLogin.Email.ToLower();
            
            var user = await _userRepo.GetUserByEmail<UserLogin>(userLogin);

            if (user == null)
            {
                return new ServiceResponse<string> { Success = false, Message = "User not found" };
            }

            if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new ServiceResponse<string> { Success = false, Message = "Password is incorrect" };
            }

            return new ServiceResponse<string> { Data = CreateToken(user), Message = "User logged in successfully" };
        }

        public async Task<ServiceResponse<bool>> ChangePassword(UserChangePassword changePassword)
        {
            var user = await _userRepo.GetUserById<UserChangePassword>(changePassword);

            if (user == null)
            {
                return new ServiceResponse<bool> { Success = false, Message = "User not found" };
            }

            CreatePasswordHash(changePassword.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userRepo.UpdateUserPassword(user);

            return new ServiceResponse<bool> { Success = true, Message = "Password changed successfully" };
        }


        private async Task<bool> UserExists(string email)
        {
            return await _userRepo.UserExists(email.ToLower());
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash =
                    hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(UserModel user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

    }
}
