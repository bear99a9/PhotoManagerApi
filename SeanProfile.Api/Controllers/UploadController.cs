using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace SeanProfile.Api.Controllers
{

    public class UploadController : BaseController
    {
        private readonly IUploadService _blogService;

        public UploadController(IUploadService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost("upload-images")]
        public async Task<IActionResult> Uploads()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var files = formCollection.Files;

                if (files.Any(f => f.Length == 0))
                {
                    throw new AppException("No files sent");
                }

                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response = await _blogService.SaveFile(files, userId);

                return Ok(response);

            }
            catch (AppException ex)
            {
                return new ValidationError(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

}
