using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SeanProfile.Api.Controllers
{

    public class PhotoController : BaseController
    {
        private readonly IPhotoService _photoService;

        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
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

                var response = await _photoService.SavePhoto(files, userId, false);

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

        [HttpPost("upload-featured-images")]
        public async Task<IActionResult> UploadFeatured()
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

                var response = await _photoService.SavePhoto(files, userId, true);

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


        [HttpGet("retrieve-all-images")]
        public async Task<IActionResult> RetrieveAll()
        {
            try
            {
                var response = await _photoService.RetrieveAllPhotos();

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

        [HttpGet("retrieve-featured-images")]
        public async Task<IActionResult> RetrieveFeatured()
        {
            try
            {
                var response = await _photoService.RetrieveFeaturedPhotos();

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

        [HttpGet("retrieve-images-co-ordinates")]
        public async Task<IActionResult> RetrieveCoOrdinates()
        {
            try
            {
                var response = await _photoService.RetrievePhotosCoOrdinates();

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
