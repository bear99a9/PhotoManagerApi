using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace SeanProfile.Api.DataLayer
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly AppSettingsModel _appSettings;
        public PhotoRepository(IOptions<AppSettingsModel> options)
        {
            _appSettings = options.Value;
        }

        private IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(_appSettings.SqlConnection);
            connection.Open();
            return connection;
        }

        public async Task InsertPhotos(List<PhotoModel> photos)
        {
            try
            {
                var sql = @"INSERT INTO photomanager_photos([photoUrl], [PermissionToView], [InsertedByUserId], [InsertedDateTime])
	                        values(@photoUrl, @PermissionToView, @InsertedByUserId, GETDATE())";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.ExecuteAsync(sql, photos);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
