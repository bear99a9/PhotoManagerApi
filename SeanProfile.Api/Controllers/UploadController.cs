using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

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
                    throw new Exception("No files sent");
                }

                var response = await _blogService.SaveFile(files);

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
