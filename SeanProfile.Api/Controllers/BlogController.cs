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
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IWebHostEnvironment env,
        ILogger<BlogController> logger)
        {
            _env = env;
            _logger = logger;
        }

            [HttpPost("upload-file"), AllowAnonymous]
            public async Task<ActionResult<IList<UploadResult>>> PostFile(
                [FromForm] IEnumerable<IFormFile> files)
            {
                var maxAllowedFiles = 3;
                long maxFileSize = 1024 * 15;
                var filesProcessed = 0;
                var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
                List<UploadResult> uploadResults = new();

                foreach (var file in files)
                {
                    var uploadResult = new UploadResult();
                    string trustedFileNameForFileStorage;
                    var untrustedFileName = file.FileName;
                    uploadResult.FileName = untrustedFileName;
                    var trustedFileNameForDisplay =
                        WebUtility.HtmlEncode(untrustedFileName);

                    if (filesProcessed < maxAllowedFiles)
                    {
                        if (file.Length == 0)
                        {
                            _logger.LogInformation("{FileName} length is 0 (Err: 1)",
                                trustedFileNameForDisplay);
                            uploadResult.ErrorCode = 1;
                        }
                        else if (file.Length > maxFileSize)
                        {
                            _logger.LogInformation("{FileName} of {Length} bytes is " +
                                "larger than the limit of {Limit} bytes (Err: 2)",
                                trustedFileNameForDisplay, file.Length, maxFileSize);
                            uploadResult.ErrorCode = 2;
                        }
                        else
                        {
                            try
                            {
                                trustedFileNameForFileStorage = file.FileName;
                                var path = Path.Combine(_env.ContentRootPath,
                                    "Images", "unsafe_uploads",
                                    trustedFileNameForFileStorage);

                                await using FileStream fs = new(path, FileMode.Create);
                                await file.CopyToAsync(fs);

                                _logger.LogInformation("{FileName} saved at {Path}",
                                    trustedFileNameForDisplay, path);
                                uploadResult.Uploaded = true;
                                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                            }
                            catch (IOException ex)
                            {
                                _logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                                    trustedFileNameForDisplay, ex.Message);
                                uploadResult.ErrorCode = 3;
                            }
                        }

                        filesProcessed++;
                    }
                    else
                    {
                        _logger.LogInformation("{FileName} not uploaded because the " +
                            "request exceeded the allowed {Count} of files (Err: 4)",
                            trustedFileNameForDisplay, maxAllowedFiles);
                        uploadResult.ErrorCode = 4;
                    }

                    uploadResults.Add(uploadResult);
                }

                return new CreatedResult(resourcePath, uploadResults);
            }
        }
    
}
