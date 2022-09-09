using SeanProfile.Api.Model;

namespace SeanProfile.Api.DataLayer
{
    public interface ITodoRepository
    {
        Task<bool> DeleteTodoById(int id);
        Task<IEnumerable<TodoModel>> GetAllTodos();
        Task<TodoModel> GetTodobyId(int id);
        Task<bool> UpdateTodoById(TodoModel todo);
        Task<bool> CreateTodo(TodoModel todo);
    }
}