
namespace SeanProfile.Api.DataLayer
{
    public interface IPhotoRepository
    {
        Task InsertPhotos(List<PhotoModel> photos);
        Task<IEnumerable<PhotoModel>> RetrieveAllPhotos();
    }
}