
namespace SeanProfile.Api.DataLayer
{
    public interface IPhotoRepository
    {
        Task InsertPhotos(List<PhotoModel> photos);
        Task<IEnumerable<PhotoModel>> RetrieveAllPhotos();
        Task<IEnumerable<PhotoModel>> RetrieveFeaturedPhotos();
        Task<IEnumerable<PhotoCoOrdinates>> RetrievePhotosCoOrdinates();
    }
}