using Dapper;
using Microsoft.Extensions.Options;
using SeanProfile.Api.Model;
using System.Data;
using System.Data.SqlClient;

namespace SeanProfile.Api.DataLayer
{
    public class UserRepository : IUserRepository
    {
        private readonly AppSettingsModel _appSettings;
        public UserRepository(IOptions<AppSettingsModel> options)
        {
            _appSettings = options.Value;
        }

        private IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_appSettings.SqlConnection);
            connection.Open();
            return connection;
        }

        public async Task<bool> UserExists(string email)
        {
            try
            {
                var sql = "SELECT Id FROM [photomanager_user] WHERE Email = @email";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.QueryAsync<UserModel>(sql, new { email });

                    return result.Any();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> InsertNewUser(UserModel user)
        {
            try
            {
                var sql = @"INSERT INTO [photomanager_user] (FirstName, LastName, Role, Email, PasswordHash, PasswordSalt)
                            OUTPUT INSERTED.Id
                        VALUES (@FirstName, @LastName, @Role, @Email, @PasswordHash, @PasswordSalt)";

                using (var connection = GetOpenConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<int>(sql, user);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserModel> GetUserByEmail<T>(T user)
        {
            try
            {
                var sql = @"SELECT * FROM [photomanager_user] WHERE Email = @Email ";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<UserModel>(sql, user);
                    return result;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserModel> GetUserById<T>(T user)
        {
            try
            {
                var sql = @"SELECT * FROM [photomanager_user] WHERE Id = @Id ";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<UserModel>(sql, user);
                    return result;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateUserPassword(UserModel user)
        {
            try
            {
                var sql = @"UPDATE [photomanager_user] SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE Id = @Id";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.ExecuteAsync(sql, user);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
