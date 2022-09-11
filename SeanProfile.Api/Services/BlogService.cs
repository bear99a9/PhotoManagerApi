namespace SeanProfile.Api.Services
{
    public class BlogService : IBlogService
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IBlobStorageRepository _blobStorageRepository;

        public BlogService(IOptions<AppSettingsModel> options,
            IBlobStorageRepository blobStorageRepository)
        {
            _appSettings = options.Value;
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
                    var trustedFileNameForDisplay = untrustedFileName;

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
