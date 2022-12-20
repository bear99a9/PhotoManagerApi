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

        public async Task InsertPasswordReset(PasswordReset passwordReset)
        {
            try
            {
                var sql = @"INSERT INTO [photomanager_passwordReset] (UserId, PasswordResetKey, PasswordResetKeyTimeOut, PasswordResetUsed, InsertedDateTime )
                        VALUES (@UserId, @PasswordResetKey, @PasswordResetKeyTimeOut, 0, GETDATE())";

                using (var connection = GetOpenConnection())
                {
                    await connection.ExecuteAsync(sql, passwordReset);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<PasswordReset> RetrievePasswordReset(string passwordResetKey)
        {
            try
            {
                var sql = @"SELECT * FROM photomanager_passwordReset WHERE PasswordResetKey = @passwordResetKey ORDER BY InsertedDateTime DESC";

                using (var connection = GetOpenConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<PasswordReset>(sql, new { passwordResetKey });

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdatePasswordReset(string passwordResetKey)
        {
            try
            {
                var sql = @"UPDATE [photomanager_passwordReset] SET PasswordResetUsed = 1, UsedDateTime = GETDATE() WHERE PasswordResetKey = @passwordResetKey";

                using (var connection = GetOpenConnection())
                {
                    await connection.ExecuteAsync(sql, new { passwordResetKey });

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public async Task<IEnumerable<UserModel>> GetAllUsersToEmail()
        {
            try
            {
                var sql = @"SELECT * FROM [photomanager_user] WHERE Role = 'Guest' ";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.QueryAsync<UserModel>(sql);
                    return result;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
