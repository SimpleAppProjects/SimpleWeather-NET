using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Firestore.v1fix.Data;
using System.Threading;

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

        private static Task<List<ImageData>> GetSnapshot()
        {
            return Task.Run(async () =>
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
            });
        }

        public static Task<ImageData> GetRandomImageForCondition(String backgroundCode)
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!await ImageDatabaseCache.IsEmpty())
                    {
                        return await ImageDatabaseCache.GetRandomImageForCondition(backgroundCode);
                    }
                    else
                    {
                        var rand = new Random();

                        var list = await GetSnapshot();

                        var imageData = list.Where(img => img.Condition == backgroundCode)
                                            .OrderBy(img => rand.Next())
                                            .Take(1)
                                            .FirstOrDefault();

                        return imageData;
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error retrieving image data");
                }

                return null;
            });
        }

        private static Task SaveSnapshot(IEnumerable<ImageData> images)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await ImageDatabaseCache.ClearCache();

                    foreach (var img in images)
                    {
                        await ImageDatabaseCache.InsertData(img);
                    }

                    // Register background task to update
#if WINDOWS_UWP && !UNIT_TEST
                    await UWP.BackgroundTasks.ImageDatabaseTask.RegisterBackgroundTask();
#endif
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error saving snapshot");
                }
            });
        }

        public static Task<long> GetLastUpdateTime()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var db = await Firebase.FirestoreHelper.GetFirestoreDB();
                    var request = db.Projects.Databases.Documents.Get(Firebase.FirestoreHelper.GetParentPath() + "/background_images_info/collection_info");
                    var authLink = await Firebase.FirebaseAuthHelper.GetAuthLink();
                    request.AddCredential(GoogleCredential.FromAccessToken(authLink.FirebaseToken));
                    using (var cts = new CancellationTokenSource(Settings.READ_TIMEOUT))
                    {
                        var doc = await request.ExecuteAsync(cts.Token);

                        if (doc.Fields.TryGetValue("last_updated", out Value value))
                        {
                            if (value.IntegerValue.HasValue)
                            {
                                return (long)value.IntegerValue;
                            }
                            else if (value.DoubleValue.HasValue)
                            {
                                return (long)value.DoubleValue.Value;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteLine(LoggerLevel.Error, e, "ImageDatabase: error querying update time");
                }

                return 0L;
            });
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

        public Task<bool> IsEmpty()
        {
            return Task.Run(async () =>
            {
                int count = await dbConnection.Table<ImageData>().CountAsync();
                return count == 0;
            });
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