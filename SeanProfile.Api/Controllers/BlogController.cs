using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SeanProfile.Api.Controllers
{

    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost("upload-file"), AllowAnonymous]
        public async Task<IActionResult> SaveFile([FromForm] IEnumerable<IFormFile> files)
        {
            try
            {

                var response = await _blogService.SaveFile(files);

                return Ok(response);
            }
            catch (AppException ex)
            {
                return new ValidationError(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }

}
