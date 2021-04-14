using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Firebase.Database.Query;

namespace SimpleWeather.WeatherData.Images
{
    public static class ImageDatabase
    {
        private static ImageDatabaseSnapshotCacheImpl sImageDBCache;
        internal static ImageDatabaseSnapshotCacheImpl ImageDatabaseCache
        {
            get
            {
                if (sImageDBCache == null)
                    sImageDBCache = new ImageDatabaseSnapshotCacheImpl();

                return sImageDBCache;
            }
        }

        public static async Task<List<ImageData>> GetAllImageDataForCondition(String backgroundCode)
        {
            List<ImageData> list = null;
            try
            {
                // Try to retrieve from cache first
                if (!ImageDataHelper.ShouldInvalidateCache)
                {
                    list = await ImageDatabaseCache.GetAllImageDataForCondition(backgroundCode);
                }
            }
            catch (Exception)
            {
                list = null;
            }

            // If data is missing from cache, get data from server
            if (list == null || list.Count == 0)
            {
                try
                {
                    var dbSnapshot = await GetSnapshot();

                    list = dbSnapshot.Where(img => img.Condition == backgroundCode)
                                     .ToList();
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error retrieving image data");
                }
            }

            return list;
        }

        public static async Task<ImageData> GetRandomImageForCondition(String backgroundCode)
        {
            ImageData imageData = null;
            try
            {
                // Try to retrieve from cache first
                if (!ImageDataHelper.ShouldInvalidateCache)
                {
                    imageData = await ImageDatabaseCache.GetRandomImageForCondition(backgroundCode);
                }
            }
            catch (Exception)
            {
                imageData = null;
            }

            // If data is missing from cache, get data from server
            if (imageData == null)
            {
                try
                {
                    var dbSnapshot = await GetSnapshot();

                    var rand = new Random();

                    imageData = dbSnapshot.Where(img => img.Condition == backgroundCode)
                                          .OrderBy(img => rand.Next())
                                          .Take(1)
                                          .FirstOrDefault();

                    return imageData;
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error retrieving image data");
                }
            }

            return imageData;
        }

        private static async Task<List<ImageData>> GetSnapshot()
        {
            var list = new List<ImageData>();

            try
            {
                var db = await Firebase.FirestoreHelper.GetFirestoreDB();
                string pageToken = null;

                do
                {
                    var request = db.Projects.Databases.Documents.List(Firebase.FirestoreHelper.GetParentPath(), "background_images");
                    var authLink = await Firebase.FirebaseAuthHelper.GetAuthLink();
                    request.AddCredential(GoogleCredential.FromAccessToken(authLink.FirebaseToken));
                    request.PageToken = pageToken;

                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    {
                        var resp = await request.ExecuteAsync(cts.Token);
                        var docs = resp.Documents;
                        list.EnsureCapacity(docs.Count);
                        foreach (var doc in docs)
                        {
                            var imgData = new ImageData();
                            foreach (var field in doc.Fields)
                            {
                                switch (field.Key)
                                {
                                    case "artistName":
                                        imgData.ArtistName = field.Value.StringValue;
                                        break;

                                    case "color":
                                        imgData.HexColor = field.Value.StringValue;
                                        break;

                                    case "condition":
                                        imgData.Condition = field.Value.StringValue;
                                        break;

                                    case "imageURL":
                                        imgData.ImageUrl = field.Value.StringValue;
                                        break;

                                    case "location":
                                        imgData.Location = field.Value.StringValue;
                                        break;

                                    case "originalLink":
                                        imgData.OriginalLink = field.Value.StringValue;
                                        break;

                                    case "siteName":
                                        imgData.SiteName = field.Value.StringValue;
                                        break;
                                }
                            }
                            list.Add(imgData);
                        }
                        pageToken = resp.NextPageToken;
                    }
                }
                while (pageToken != null);

                await SaveSnapshot(list);
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error retrieving image data");
            }

            return list;
        }

        private static async Task SaveSnapshot(IEnumerable<ImageData> images)
        {
            AnalyticsLogger.LogEvent("ImageDatabase: SaveSnapshot");

            try
            {
                await ImageDatabaseCache.ClearCache();

                foreach (var img in images)
                {
                    await ImageDatabaseCache.InsertData(img);
                }

                ImageDataHelper.ShouldInvalidateCache = false;
                if (ImageDataHelper.ImageDBUpdateTime == 0)
                {
                    ImageDataHelper.ImageDBUpdateTime = await ImageDatabase.GetLastUpdateTime();
                }

                // Register background task to update
#if WINDOWS_UWP && !UNIT_TEST
                SimpleLibrary.GetInstance().RequestAction(CommonActions.ACTION_IMAGES_UPDATETASK);
#endif
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error saving snapshot");
            }
        }

        public static async Task<long> GetLastUpdateTime()
        {
            try
            {
                var db = await Firebase.FirebaseDatabaseHelper.GetFirebaseDatabase();
                var last_updated = await db.Child("background_images_info")
                    .Child("collection_info")
                    .Child("last_updated")
                    .OnceSingleAsync<long>(TimeSpan.FromMilliseconds(Settings.READ_TIMEOUT));

                return last_updated;
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error querying update time");
            }

            return 0L;
        }
    }

    public sealed partial class ImageDatabaseSnapshotCacheImpl
    {
        private SQLite.SQLiteAsyncConnection dbConnection;

        public ImageDatabaseSnapshotCacheImpl()
        {
            dbConnection = new SQLite.SQLiteAsyncConnection(imageDBPath,
                SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.FullMutex);

            var conn = dbConnection.GetConnection();
            var _lock = conn.Lock();
            conn.BusyTimeout = TimeSpan.FromSeconds(5);
            conn.EnableWriteAheadLogging();
            conn.CreateTable<ImageData>();
            _lock.Dispose();
        }

        public async Task<bool> IsEmpty()
        {
            int count = await dbConnection.Table<ImageData>().CountAsync();
            return count == 0;
        }

        // Select
        public Task<List<ImageData>> GetAllImageData()
        {
            return dbConnection.Table<ImageData>().ToListAsync();
        }

        public Task<List<ImageData>> GetAllImageDataForCondition(String backgroundCode)
        {
            return dbConnection.Table<ImageData>()
                               .Where(i => Equals(i.Condition, backgroundCode))
                               .ToListAsync();
        }

        public Task<ImageData> GetRandomImageForCondition(String backgroundCode)
        {
            // TODO: change if we end up having alot more items
            // This is ok for now as table size will be relatively low
            return dbConnection.FindWithQueryAsync<ImageData>("SELECT * FROM imagedata WHERE condition = ? ORDER BY RANDOM() LIMIT 1", backgroundCode);
        }

        // Insert
        public Task InsertData(ImageData imageData)
        {
            return dbConnection.InsertAsync(imageData);
        }

        // Delete
        public Task ClearCache()
        {
            return dbConnection.DeleteAllAsync<ImageData>();
        }
    }
}