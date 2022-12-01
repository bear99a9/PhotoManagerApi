using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace SeanProfile.Api.Controllers
{

    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost("upload"), AllowAnonymous]
        public async Task<IActionResult> upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok(new { dbPath });
                }
                else
                {
                    throw new Exception("Too many files");
                }
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

        [HttpPost(), AllowAnonymous]
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
                return BadRequest(ex.Message);
            }

        }
    }

}
