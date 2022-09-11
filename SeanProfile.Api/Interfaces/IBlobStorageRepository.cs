using Azure.Storage.Blobs;

namespace SeanProfile.Api.DataLayer
{
    public interface IBlobStorageRepository
    {
        BlobContainerClient ConnectionStringAsync();
        Task<string> UploadBinary(BlobContainerClient container, IFormFile file, string fileName);
    }
}