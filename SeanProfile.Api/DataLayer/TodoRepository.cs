using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;

namespace SeanProfile.Api.DataLayer
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppSettingsModel _appSettings;

        public TodoRepository(IOptions<AppSettingsModel> options)
        {
            _appSettings = options.Value;
        }

        private IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_appSettings.SqlConnection);
            connection.Open();
            return connection;
        }

        public async Task<IEnumerable<TodoModel>> GetAllTodos()
        {
            try
            {
                var sql = "SELECT * FROM todo";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.QueryAsync<TodoModel>(sql);

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<TodoModel> GetTodobyId(int id)
        {
            try
            {
                var sql = "SELECT * FROM todo where Id = @id";

                using (var connection = GetOpenConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<TodoModel>(sql, new { id });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> CreateTodo(TodoModel todo)
        {
            try
            {
                var sql = "INSERT INTO todo(Title, Status, DueDate, IsCompleted) VALUES(@Title, @Status, @DueDate, @IsCompleted)";
                using (var connection = GetOpenConnection())
                {
                    var result = await connection.ExecuteAsync(sql, new { todo.Title, todo.Status, todo.DueDate, todo.IsCompleted });

                    return result != 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> UpdateTodoById(TodoModel todo)
        {
            try
            {
                var sql = "UPDATE todo SET Title = @Title, Status = @Status, IsCompleted = @IsCompleted, DueDate = @DueDate WHERE Id = @Id";
                using (var connection = GetOpenConnection())
                {
                    var result = await connection.ExecuteAsync(sql, new { todo.Id, todo.Title, todo.Status, todo.DueDate, todo.IsCompleted });

                    return result != 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteTodoById(int id)
        {
            try
            {
                var sql = "DELETE FROM todo WHERE Id = @id";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.ExecuteAsync(sql, new { id });

                    return result != 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
