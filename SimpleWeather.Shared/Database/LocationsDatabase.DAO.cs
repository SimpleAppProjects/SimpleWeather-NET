using SimpleWeather.LocationData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleWeather.Database
{
    public partial class LocationsDatabase
    {
        #region LocationData
        public Task InsertLocationData(LocationData.LocationData location)
        {
            return _database.InsertOrReplaceAsync(location);
        }

        public Task UpdateLocationData(LocationData.LocationData location)
        {
            return _database.UpdateAsync(location);
        }

        public Task DeleteLocationData(LocationData.LocationData location)
        {
            return _database.DeleteAsync(location);
        }

        public Task DeleteAllLocationData()
        {
            return _database.DeleteAllAsync<LocationData.LocationData>();
        }

        public Task DeleteLocationDataByKey(string key)
        {
            return _database.DeleteAsync<LocationData.LocationData>(key);
        }

        public Task<List<LocationData.LocationData>> LoadAllLocationData()
        {
            return _database.Table<LocationData.LocationData>().ToListAsync();
        }

        public Task<List<LocationData.LocationData>> GetFavorites()
        {
            return _database.QueryAsync<LocationData.LocationData>(
                "select locations.* from locations" +
                " INNER JOIN favorites on locations.query = favorites.query" +
                " ORDER by favorites.position"
            );
        }

        public Task<LocationData.LocationData> GetFirstFavorite()
        {
            return _database.FindWithQueryAsync<LocationData.LocationData>(
                "select locations.* from locations" +
                " INNER JOIN favorites on locations.query = favorites.query" +
                " ORDER by favorites.position LIMIT 1"
            );
        }

        public Task<LocationData.LocationData> GetLocation(string query)
        {
            return _database.FindAsync<LocationData.LocationData>(query);
        }

        public Task<int> GetLocationCount()
        {
            return _database.Table<LocationData.LocationData>().CountAsync();
        }
        #endregion

        #region Favorites
        public Task InsertFavorite(Favorites favorite)
        {
            return _database.InsertOrReplaceAsync(favorite);
        }

        public Task UpdateFavorite(Favorites favorite)
        {
            return _database.UpdateAsync(favorite);
        }

        public Task DeleteFavoriteData(Favorites favorite)
        {
            return _database.DeleteAsync(favorite);
        }

        public Task DeleteAllFavoriteData()
        {
            return _database.DeleteAllAsync<Favorites>();
        }

        public Task DeleteFavoritesByKey(string key)
        {
            return _database.DeleteAsync<Favorites>(key);
        }

        public Task<List<Favorites>> LoadAllFavorites()
        {
            return _database.Table<Favorites>().ToListAsync();
        }

        public Task<Favorites> GetFavorite(string query)
        {
            return _database.FindAsync<Favorites>(query);
        }

        public Task<List<Favorites>> LoadAllFavoritesByPosition()
        {
            return _database.Table<Favorites>()
                            .OrderBy(f => f.position)
                            .ToListAsync();
        }

        public Task<int> GetFavoritesCount()
        {
            return _database.Table<Favorites>().CountAsync();
        }

        public Task UpdateFavPosition(string key, int toPos)
        {
            return _database.ExecuteAsync("update favorites set position = ? where query = ?", toPos, key);
        }
        #endregion
    }
}
