using Microsoft.Extensions.Options;
using System.Net;

namespace SeanProfile.Api.Services
{
    public class BlogService : IBlogService
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IWebHostEnvironment _env;
        private readonly BlobStorageRepository _blobStorageRepository;

        public BlogService(IOptions<AppSettingsModel> options, IWebHostEnvironment env,
            BlobStorageRepository blobStorageRepository)
        {
            _appSettings = options.Value;
            _env = env;
            _blobStorageRepository = blobStorageRepository;
        }

        public async Task<ServiceResponse<IList<UploadResult>>> SaveFile(IEnumerable<IFormFile> files)
        {
            try
            {
                var filesProcessed = 0;
                List<UploadResult> uploadResults = new();

                foreach (var file in files)
                {
                    var uploadResult = new UploadResult();
                    string trustedFileNameForFileStorage;
                    var untrustedFileName = file.FileName;
                    uploadResult.FileName = untrustedFileName;
                    var trustedFileNameForDisplay =
                        WebUtility.HtmlEncode(untrustedFileName);

                    if (filesProcessed < _appSettings.MaxAllowedFiles)
                    {
                        if (file.Length == 0)
                        {
                            uploadResult.ErrorCode = 1;
                        }
                        else if (file.Length > _appSettings.MaxFileSize)
                        {
                            uploadResult.ErrorCode = 2;
                        }
                        else
                        {
                            try
                            {
                                var pictureType = file.FileName.Split(".").LastOrDefault();

                                if (!string.IsNullOrWhiteSpace(pictureType))
                                {

                                    trustedFileNameForFileStorage = $"{Guid.NewGuid()}.{pictureType}";
                                    var path = Path.Combine(_env.ContentRootPath,
                                        "Images", "unsafe_uploads",
                                        trustedFileNameForFileStorage);

                                    await using FileStream fs = new(path, FileMode.Create);
                                    await file.CopyToAsync(fs);

                                    var blobContainerClient = _blobStorageRepository.ConnectionStringAsync();
                                    var blobUri = await _blobStorageRepository.UploadBinary(blobContainerClient, file, trustedFileNameForFileStorage, pictureType);
                                    uploadResult.Uploaded = true;
                                    uploadResult.StoredFileName = trustedFileNameForFileStorage;
                                }
                            }
                            catch (IOException)
                            {
                                uploadResult.ErrorCode = 3;
                            }
                        }

                        filesProcessed++;
                    }
                    else
                    {
                        uploadResult.ErrorCode = 4;
                    }

                    uploadResults.Add(uploadResult);
                }

                // var response = new CreatedResult(resourcePath, uploadResults);
                var response = new ServiceResponse<IList<UploadResult>>()
                {
                    Data = uploadResults,
                    Success = true,
                    Message = "File has been uploaded"
                };

                return response;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
