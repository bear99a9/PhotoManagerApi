
namespace SeanProfile.Api.Services
{
    public interface ITodoService
    {
        Task<bool> CreateTodo(TodoModel request);
        Task<bool> DeleteTodoById(int id);
        Task<IEnumerable<TodoModel>> GetAllTodos();
        Task<TodoModel> GetTodoById(int id);
        Task<bool> UpdateTodoById(TodoModel request);
    }
}