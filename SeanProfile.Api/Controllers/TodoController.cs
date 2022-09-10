using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SeanProfile.Api.Controllers
{
    public class TodoController : BaseController
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }
        
        [HttpGet("GetAllTodos"), AllowAnonymous]
        public async Task<IActionResult> GetAllTodos()
        {
            try
            {
                var response = await _todoService.GetAllTodos();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTodoById/{id}")]
        public async Task<IActionResult> GetTodoById(int id)
        {
            try
            {
                var response = await _todoService.GetTodoById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteTodoById/{id}")]
        public async Task<IActionResult> DeleteTodoById(int id)
        {
            try
            {
                var response = await _todoService.DeleteTodoById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateTodoById")]
        public async Task<IActionResult> UpdateTodoById(TodoModel request)
        {
            try
            {
                var response = await _todoService.UpdateTodoById(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateTodo")]
        public async Task<IActionResult> CreateTodo(TodoModel request)
        {
            try
            {
                var response = await _todoService.CreateTodo(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
