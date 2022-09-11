using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            var response = await _blogService.SaveFile(files);

            return Ok(response);
        }
    }

}
