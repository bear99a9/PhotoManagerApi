using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace SeanProfile.Api.DataLayer
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly AppSettingsModel _appSettings;

        public BlobStorageRepository(IOptions<AppSettingsModel> options)
        {
            _appSettings = options.Value;
        }

        public BlobContainerClient ConnectionStringAsync()
        {
            try
            {

                // Create a client that can authenticate with a connection string
                BlobServiceClient service = new(_appSettings.BlobConnString);

                var client = service.GetBlobContainerClient(_appSettings.BlobContainer);

                return client;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<string> UploadBinary(BlobContainerClient container, IFormFile file, string fileName, string contentType)
        {
            try
            {
                BlobClient blobClient = container.GetBlobClient(fileName);

                Stream stream = file.OpenReadStream();//new MemoryStream(document.FileData);

                await blobClient.UploadAsync(
                    stream,
                    new BlobHttpHeaders
                    {
                        ContentType = "application/pdf"
                    });

                var uri = GetServiceSasUriForBlob(blobClient);

                return uri.ToString();
            }
            catch (Exception)
            {
                throw;
            }


        }

        private static Uri GetServiceSasUriForBlob(BlobClient blobClient, string? storedPolicyName = null)
        {
            try
            {

                // Check whether this BlobClient object has been authorized with Shared Key.
                if (blobClient.CanGenerateSasUri)
                {
                    // Create a SAS token that's valid for one hour.
                    BlobSasBuilder sasBuilder = new()
                    {
                        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                        BlobName = blobClient.Name,
                        Resource = "b"
                    };

                    if (storedPolicyName == null)
                    {
                        sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddYears(100);
                        sasBuilder.SetPermissions(BlobSasPermissions.Read |
                            BlobSasPermissions.Write);
                    }
                    else
                    {
                        sasBuilder.Identifier = storedPolicyName;
                    }

                    Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

                    return sasUri;
                }
                else
                {
                    throw new Exception(@"BlobClient must be authorized with Shared Key
                          credentials to create a service SAS.");
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

    }
}
