namespace SeanProfile.Api.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IBlobStorageRepository _blobStorageRepository;
        private readonly IPhotoRepository _photoRepository;

        public PhotoService(IOptions<AppSettingsModel> options,
            IBlobStorageRepository blobStorageRepository, IPhotoRepository photoRepository)
        {
            _appSettings = options.Value;
            _blobStorageRepository = blobStorageRepository;
            _photoRepository = photoRepository;
        }

        public async Task<ServiceResponseModel<IList<string>>> SavePhoto(IEnumerable<IFormFile> files, int userId)
        {
            try
            {
                List<PhotoModel> photos = new();
                List<string> urls = new();
                var blobContainerClient = _blobStorageRepository.ConnectionStringAsync();

                foreach (var file in files)
                {
                    string trustedFileNameForFileStorage;
                    string untrustedFileName = file.FileName;
                    string trustedFileNameForDisplay = untrustedFileName;

                    if (file.Length == 0)
                    {
                        throw new AppException("File length is 0");
                    }
                    else
                    {
                        try
                        {
                            var pictureType = file.FileName.Split(".").LastOrDefault();

                            if (!string.IsNullOrWhiteSpace(pictureType))
                            {
                                trustedFileNameForFileStorage = $"{Guid.NewGuid()}.{pictureType}";

                                var blobUri = await _blobStorageRepository.UploadBinary(blobContainerClient, file, trustedFileNameForFileStorage, pictureType);

                                urls.Add(blobUri);

                                photos.Add(new()
                                {
                                    PhotoUrl = blobUri,
                                    PermissionToView = false,
                                    InsertedByUserId = userId,
                                });
                            }
                        }
                        catch (IOException ex)
                        {
                            throw new AppException(ex.Message);
                        }
                    }
                }

                await _photoRepository.InsertPhotos(photos);

                var message = files.Count().Equals(1) ?
                    "The image has been successfully uploaded" : "The images have been successfully uploaded";

                var response = new ServiceResponseModel<IList<string>>()
                {
                    Data = urls,
                    Success = true,
                    Message = message
                };

                return response;
            }
            catch (AppException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
