using System.Drawing;
using System.Drawing.Imaging;

namespace SeanProfile.Api.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly AppSettingsModel _appSettings;
        private readonly IBlobStorageRepository _blobStorageRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public PhotoService(IOptions<AppSettingsModel> options,
            IBlobStorageRepository blobStorageRepository, IPhotoRepository photoRepository,
            IEmailService emailService, IUserRepository userRepository)
        {
            _appSettings = options.Value;
            _blobStorageRepository = blobStorageRepository;
            _photoRepository = photoRepository;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponseModel<IList<string>>> SavePhoto(IEnumerable<IFormFile> files, int userId, bool permissionToView)
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

                                GetLatitudeAndlongitude(file.OpenReadStream(), out double latitude, out double longitude);

                                photos.Add(new()
                                {
                                    PhotoUrl = blobUri,
                                    PermissionToView = permissionToView,
                                    InsertedByUserId = userId,
                                    PhotoName = trustedFileNameForFileStorage,
                                    PhotoThumb = $"{_appSettings.ImagekitURL}{trustedFileNameForFileStorage}{_appSettings.ThumbNailSize}",
                                    PhotoSRC = $"{_appSettings.ImagekitURL}{trustedFileNameForFileStorage}{_appSettings.SRCSize}",
                                    Latitude = latitude,
                                    Longitude = longitude,
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

                await SendEmail();

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<ServiceResponseModel<IEnumerable<PhotoModel>>> RetrieveAllPhotos()
        {
            try
            {
                var photos = await _photoRepository.RetrieveAllPhotos();

                var response = new ServiceResponseModel<IEnumerable<PhotoModel>>()
                {
                    Data = photos,
                    Success = true,
                    Message = "Here are the photos"
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

        public async Task<ServiceResponseModel<IEnumerable<PhotoModel>>> RetrieveFeaturedPhotos()
        {
            try
            {
                var photos = await _photoRepository.RetrieveFeaturedPhotos();

                var response = new ServiceResponseModel<IEnumerable<PhotoModel>>()
                {
                    Data = photos,
                    Success = true,
                    Message = "Here are the photos"
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
        private async Task SendEmail()
        {
            try
            {
                var users = await _userRepository.GetAllUsersToEmail();

                await _emailService.SendNewUploadEmail(users);
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }

        }

        private static void GetLatitudeAndlongitude(Stream fileStream, out double latitude, out double longitude)
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(fileStream))
                {
                    latitude = GetCoordinateDouble(bitmap.PropertyItems.Single(p => p.Id == 1).Value[0], bitmap.PropertyItems.Single(p => p.Id == 2));
                    longitude = GetCoordinateDouble(bitmap.PropertyItems.Single(p => p.Id == 3).Value[0], bitmap.PropertyItems.Single(p => p.Id == 4));
                }
            }
            catch (Exception)
            {
                latitude = 0;
                longitude = 0;
            }
        }

        private static double GetCoordinateDouble(byte gpsRef, PropertyItem propItem)
        {
            try
            {
                uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
                uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
                double degrees = degreesNumerator / (double)degreesDenominator;

                uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
                uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
                double minutes = minutesNumerator / (double)minutesDenominator;

                uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
                uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
                double seconds = secondsNumerator / (double)secondsDenominator;

                double coorditate = degrees + (minutes / 60d) + (seconds / 3600d);

                if (gpsRef == 'S' || gpsRef == 'W')
                {
                    coorditate *= -1;
                }
                return coorditate;
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
