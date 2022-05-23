using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeanProfile.Api.DataLayer;
using SeanProfile.Api.Model;
using Microsoft.AspNetCore.Authorization;

namespace SeanProfile.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        
        [HttpGet("GetAllTodos"), AllowAnonymous]
        public async Task<IActionResult> GetAllTodos()
        {
            try
            {
                var response = await _todoRepository.GetAllTodos();
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
                var response = await _todoRepository.GetTodobyId(id);
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
                var response = await _todoRepository.DeleteTodoById(id);
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
                var response = await _todoRepository.UpdateTodoById(request);
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
                var response = await _todoRepository.CreateTodo(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}
