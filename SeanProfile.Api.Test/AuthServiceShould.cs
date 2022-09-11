using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using SeanProfile.Api.DataLayer;
using SeanProfile.Api.Model;
using SeanProfile.Api.Services;
using System.Threading.Tasks;
using Xunit;

namespace SeanProfile.Api.Test
{
    public class AuthServiceShould
    {
        private readonly IConfigurationRoot _configuration;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly AuthService _sut;
        private readonly Fixture _fixture;
        private readonly Mock<IOptions<AppSettingsModel>> _appsettings;

        public AuthServiceShould()
        {

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();
            _appsettings = new Mock<IOptions<AppSettingsModel>>();
            _appsettings.Setup(x => x.Value).Returns(new AppSettingsModel
            {
                SqlConnection = _configuration.GetValue<string>("AppSettings:SqlConnection"),
                BlobConnString = _configuration.GetValue<string>("AppSettings:BlobConnString"),
                BlobContainer = _configuration.GetValue<string>("AppSettings:BlobContainer"),
                Token = _configuration.GetValue<string>("AppSettings:Token")
            });
            _mockUserRepo = new Mock<IUserRepository>();
            _fixture = new Fixture();
            _sut = new AuthService(_mockUserRepo.Object, _appsettings.Object);
        }

        [Fact]
        public async Task NotRegisterExsitingUser()
        {
            // Assign
            var user = _fixture.Build<UserModel>().With(x => x.Email,"test@test.com").Create();

            _mockUserRepo.Setup(x => x.UserExists(user.Email)).ReturnsAsync(true);

            // Act
            var actual = await _sut.Register(user);

            var expected = new ServiceResponse<int>()
            {
                Success = false,
                Message = "Email address already exists"
            };

            //Assert
            Assert.Equal(expected.Message, actual.Message);
            Assert.False(actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Fact]
        public async Task RegisterANewUser()
        {
            // Assign
            var user = _fixture.Build<UserModel>()
                .Without(x => x.PasswordSalt)
                .Without(x => x.PasswordHash)
                .Without(x => x.TokenExpires)
                .Without(x => x.TokenCreated)
                .Without(x => x.RefreshToken)
                .Without(x => x.Id)
                .Create();

            _mockUserRepo.Setup(x => x.UserExists(user.Email)).ReturnsAsync(false);
            _mockUserRepo.Setup(x => x.InsertNewUser(user)).ReturnsAsync(3);

            // Act
            var actual = await _sut.Register(user);

            var expected = new ServiceResponse<int>()
            {
                Data = 3,
                Success = true,
                Message = "User added successfully"
            };

            //Assert
            Assert.Equal(expected.Message, actual.Message);
            Assert.True(actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Fact]
        public async Task LoginRegisteredUsersWithCorrectPassword()
        {
            // Assign
            UserLogin userLogin = new()
            {
                Email = "test@test.com",
                Password = "test123",
            };
                
            var userRepo = new UserRepository(_appsettings.Object);

            var sut = new AuthService(userRepo, _appsettings.Object);
            // Act
            var actual = await sut.Login(userLogin);

            var expected = new ServiceResponse<string>()
            {
                Success = true,
                Message = "User logged in successfully"
            };

            //Assert
            Assert.Equal(expected.Message, actual.Message);
            Assert.True(actual.Success);
            Assert.IsType<string>(actual.Data);
        }

        [Fact]
        public async Task NotLoginRegisteredUsersWithIncorrectPasswords()
        {
            // Assign
            UserLogin userLogin = new()
            {
                Email = "test@test.com",
                Password = "test123NotCorrect",
            };

            var userRepo = new UserRepository(_appsettings.Object);

            var sut = new AuthService(userRepo, _appsettings.Object);
            // Act
            var actual = await sut.Login(userLogin);

            var expected = new ServiceResponse<string>()
            {
                Success = false,
                Message = "Password is incorrect"
            };

            //Assert
            Assert.Equal(expected.Message, actual.Message);
            Assert.False(actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Fact]
        public async Task NotLoginUnRegisteredUsers()
        {
            // Assign
            UserLogin userLogin = new()
            {
                Email = "test1234556677@test.com",
                Password = "test123NotCorrect",
            };

            var userRepo = new UserRepository(_appsettings.Object);

            var sut = new AuthService(userRepo, _appsettings.Object);
            // Act
            var actual = await sut.Login(userLogin);

            var expected = new ServiceResponse<string>()
            {
                Success = false,
                Message = "User not found"
            };

            //Assert
            Assert.Equal(expected.Message, actual.Message);
            Assert.False(actual.Success);
            Assert.Equal(expected.Data, actual.Data);
        }

        [Fact]
        public async Task NotChangePasswordsOfUnRegisteredUsers()
        {
            // Assign
            var changePassword = _fixture.Create<UserChangePassword>();

            _mockUserRepo.Setup(x => x.GetUserById<UserChangePassword>(changePassword)).ReturnsAsync(() => null);

            // Act
            var actual = await _sut.ChangePassword(changePassword);

            var expected = new ServiceResponse<string>()
            {
                Success = false,
                Message = "User not found"
            };

            //Assert
            Assert.Equal(expected.Message, actual.Message);
            Assert.False(actual.Success);
        }

        [Fact]
        public async Task ChangePasswordsOfRegisteredUsers()
        {
            // Assign
            var changePassword = _fixture.Create<UserChangePassword>();

            var user = _fixture.Create<UserModel>();

            _mockUserRepo.Setup(x => x.GetUserById<UserChangePassword>(changePassword)).ReturnsAsync(() => user);

            // Act
            var actual = await _sut.ChangePassword(changePassword);

            var expected = new ServiceResponse<string>()
            {
                Success = true,
                Message = "Password changed successfully"
            };

            //Assert
            Assert.Equal(expected.Message, actual.Message);
            Assert.True(actual.Success);
        }

    }
}