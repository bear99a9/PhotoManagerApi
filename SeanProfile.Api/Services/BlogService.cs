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

        public async Task<ServiceResponse<IList<string>>> SaveFile(IEnumerable<IFormFile> files)
        {
            try
            {
                var filesProcessed = 0;
                List<BlogModel> uploadResults = new();
                List<string> urls = new();
                var blobContainerClient = _blobStorageRepository.ConnectionStringAsync();

                foreach (var file in files)
                {
                    var uploadResult = new BlogModel();
                    string trustedFileNameForFileStorage;
                    var untrustedFileName = file.FileName;
                    var trustedFileNameForDisplay = untrustedFileName;

                    if (file.Length == 0)
                    {
                        throw new Exception("File length is 0");
                    }
                    //else if (file.Length > _appSettings.MaxFileSize)
                    //{
                    //    throw new Exception("File size is over max");
                    //}
                    else
                    {
                        try
                        {
                            var pictureType = file.FileName.Split(".").LastOrDefault();

                            if (!string.IsNullOrWhiteSpace(pictureType))
                            {
                                trustedFileNameForFileStorage = $"{Guid.NewGuid()}.{pictureType}";

                                var blobUri = await _blobStorageRepository.UploadBinary(blobContainerClient, file, trustedFileNameForFileStorage, pictureType);

                                uploadResult.ImageUrl = blobUri;
                                urls.Add(blobUri);  
                            }
                        }
                        catch (IOException)
                        {
                            throw;
                        }
                        filesProcessed++;

                    }

                    uploadResults.Add(uploadResult);
                }

                var response = new ServiceResponse<IList<string>>()
                {
                    Data = urls,
                    Success = true,
                    Message = "Files has been uploaded"
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
