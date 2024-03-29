﻿using SeanProfile.Api.Model;

namespace SeanProfile.Api.Services
{
    public interface IAuthService
    {
        Task<ServiceResponseModel<int>> Register(UserModel user);
        Task<ServiceResponseModel<string>> Login(UserLogin userLogin);
        Task<ServiceResponseModel<bool>> ChangePassword(UserChangePassword changePassword);
        Task<ServiceResponseModel<bool>> RequestPasswordReset(UserRequestPasswordReset userRequestChangePassword);
        Task<bool> IsAccessTokenAuthorised(UserChangePassword request);
    }
}