﻿using Dapper;
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
                var sql = @"INSERT INTO photomanager_photos([photoUrl],[PermissionToView],[InsertedByUserId],[InsertedDateTime],
                            [PhotoName],[PhotoThumb],[PhotoSRC])
	                        values(@photoUrl, @PermissionToView, @InsertedByUserId, GETDATE(),@PhotoName,@PhotoThumb,@PhotoSRC)";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.ExecuteAsync(sql, photos);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<PhotoModel>> RetrieveAllPhotos()
        {
            try
            {
                var sql = @"SELECT * FROM photomanager_photos";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.QueryAsync<PhotoModel>(sql);

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<PhotoModel>> RetrieveFeaturedPhotos()
        {
            try
            {
                var sql = @"SELECT * FROM photomanager_photos WHERE PermissionToView = 1";

                using (var connection = GetOpenConnection())
                {
                    var result = await connection.QueryAsync<PhotoModel>(sql);

                    return result;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
