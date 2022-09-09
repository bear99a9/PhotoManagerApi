using Dapper;
using SeanProfile.Api.Model;
using System.Data;
using System.Data.SqlClient;

namespace SeanProfile.Api.DataLayer
{
    public class UserRepository : IUserRepository
    {
        private string _ConnString;

        public UserRepository()
        {
            _ConnString = "Server = (localdb)\\mssqllocaldb; Database = SeanProfileDB; Trusted_Connection = True; MultipleActiveResultSets = true";
        }

        private IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_ConnString);
            connection.Open();
            return connection;
        }

        public async Task<bool> UserExists(string email)
        {
            try
            {
                var sql = "SELECT Id FROM [user] WHERE Email = @email";

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
                var sql = @"INSERT INTO [user] (Username, FirstName, LastName, Role, Email, PasswordHash, PasswordSalt)
                            OUTPUT INSERTED.Id
                        VALUES (@Username, @FirstName, @LastName, @Role, @Email, @PasswordHash, @PasswordSalt)";

                using (var connection = GetOpenConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<int>(sql, user);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<UserModel> GetUserByEmail<T>(T user)
        {
            try
            {
                var sql = @"SELECT * FROM [user] WHERE Email = @Email ";

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
                var sql = @"SELECT * FROM [user] WHERE Id = @Id ";

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
                var sql = @"UPDATE [user] SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE Id = @Id";

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
