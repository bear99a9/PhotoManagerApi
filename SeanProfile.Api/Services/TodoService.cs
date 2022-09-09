namespace SeanProfile.Api.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;

        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<IEnumerable<TodoModel>> GetAllTodos()
        {
            try
            {
                return await _todoRepository.GetAllTodos();
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
                return await _todoRepository.DeleteTodoById(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TodoModel> GetTodoById(int id)
        {
            try
            {
                return await _todoRepository.GetTodobyId(id);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateTodoById(TodoModel request)
        {
            try
            {
                return await _todoRepository.UpdateTodoById(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CreateTodo(TodoModel request)
        {
            try
            {
                return await _todoRepository.CreateTodo(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
