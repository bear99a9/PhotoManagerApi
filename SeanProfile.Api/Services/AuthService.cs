﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly AppSettingsModel _appSettings;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IOptions<AppSettingsModel> options, IEmailService emailService)
        {
            _appSettings = options.Value;
            _userRepo = userRepository;
            _emailService = emailService;
        }


        public async Task<ServiceResponseModel<int>> Register(UserModel user)
        {
            try
            {
                var isUserRegistered = await _userRepo.UserExists(user.Email.ToLower());
                if (isUserRegistered)
                {
                    return new ServiceResponseModel<int> { Success = false, Message = "Email address already exists" };
                }

                CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Role = "Guest";
                user.Email = user.Email.ToLower();

                user.Id = await _userRepo.InsertNewUser(user);

                return new ServiceResponseModel<int> { Data = user.Id, Message = "User added successfully" };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ServiceResponseModel<string>> Login(UserLogin userLogin)
        {
            try
            {

                userLogin.Email = userLogin.Email.ToLower();

                var user = await _userRepo.GetUserByEmail<UserLogin>(userLogin);

                if (user == null)
                {
                    return new ServiceResponseModel<string> { Success = false, Message = "User not found" };
                }

                if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return new ServiceResponseModel<string> { Success = false, Message = "Password is incorrect" };
                }

                return new ServiceResponseModel<string> { Data = CreateToken(user), Message = "User logged in successfully" };
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<ServiceResponseModel<bool>> ChangePassword(UserChangePassword changePassword)
        {
            try
            {

                var user = await _userRepo.GetUserById<UserChangePassword>(changePassword);

                if (user == null)
                {
                    return new ServiceResponseModel<bool> { Success = false, Message = "User not found" };
                }

                CreatePasswordHash(changePassword.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                await _userRepo.UpdateUserPassword(user);

                return new ServiceResponseModel<bool> { Success = true, Message = "Password reset successfully" };
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<ServiceResponseModel<bool>> RequestPasswordReset(UserRequestPasswordReset userRequestChangePassword)
        {
            try
            {
                userRequestChangePassword.Email = userRequestChangePassword.Email.ToLower();

                var user = await _userRepo.GetUserByEmail<UserRequestPasswordReset>(userRequestChangePassword);

                if (user == null)
                {
                    return new ServiceResponseModel<bool> { Success = false, Message = "Looks like you have enetered the wrong email address. Please enter the email address you registered with." };
                }

                var passwordReset = new PasswordReset
                {
                    PasswordResetKey = $"{Guid.NewGuid()}{Guid.NewGuid()}",
                    PasswordResetKeyTimeOut = DateTime.Now.AddHours(_appSettings.PasswordResetHoursValidFor).Ticks,
                    UserId = user.Id
                };

                await _emailService.SendPasswordResetEmail(user, passwordReset);

                await _userRepo.InsertPasswordReset(passwordReset);

                return new ServiceResponseModel<bool> { Success = true, Message = "An email has been sent to your inbox to reset your password" };
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<bool> IsAccessTokenAuthorised(UserChangePassword request)
        {
            try
            {
                var roomUserAccess = await _userRepo.RetrievePasswordReset(request.PasswordResetKey);

                if (roomUserAccess is null) return false;

                if (DateTime.Now.Ticks > roomUserAccess.PasswordResetKeyTimeOut) return false;

                await _userRepo.UpdatePasswordReset(request.PasswordResetKey);

                request.Id = roomUserAccess.UserId;
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }



        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
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
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("email", user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim("role", user.Role),
                new Claim("id", user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_appSettings.Token));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(_appSettings.BearerTokenHoursValidFor),
                    signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private static RefreshToken GenerateRefreshToken()
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
