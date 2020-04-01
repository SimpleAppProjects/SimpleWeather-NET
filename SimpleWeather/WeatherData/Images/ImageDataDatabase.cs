using SimpleWeather.WeatherData.Images.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Task<List<ImageData>> GetAllImageDataForCondition(String backgroundCode)
        {
            return Task.Run(async () =>
            {
                var list = new List<ImageData>();

                if (!await ImageDatabaseCache.IsEmpty())
                {
                    return await ImageDatabaseCache.GetAllImageData();
                }
                else
                {
                    var db = await Firebase.FirestoreHelper.GetFirestoreDB();
                    await db.Collection("background_images")
                            .WhereEqualTo("condition", backgroundCode)
                            .StreamAsync()
                            .ForEachAsync((docSnapshot) =>
                            {
                                if (docSnapshot.Exists)
                                    list.Add(docSnapshot.ConvertTo<ImageData>());
                            }).ConfigureAwait(false);

                    await SaveSnapshot(db);
                }

                return list;
            });
        }

        public static Task<ImageData> GetRandomImageForCondition(String backgroundCode)
        {
            return Task.Run(async () =>
            {
                if (!await ImageDatabaseCache.IsEmpty())
                {
                    return await ImageDatabaseCache.GetRandomImageForCondition(backgroundCode);
                }
                else
                {
                    var db = await Firebase.FirestoreHelper.GetFirestoreDB();

                    var rand = new Random();

                    var imageData = await db.Collection("background_images")
                                            .WhereEqualTo("condition", backgroundCode)
                                            .StreamAsync()
                                            .OrderBy(doc => rand.Next())
                                            .Take(1)
                                            .Select((docSnapshot) =>
                                            {
                                                return docSnapshot.ConvertTo<ImageData>();
                                            })
                                            .FirstOrDefault()
                                            .ConfigureAwait(false);

                    await SaveSnapshot(db);

                    return imageData;
                }
            });
        }

        private static Task SaveSnapshot(Google.Cloud.Firestore.FirestoreDb firestoreDb)
        {
            return Task.Run(async () =>
            {
                await ImageDatabaseCache.ClearCache();

                await firestoreDb.Collection("background_images")
                                 .StreamAsync()
                                 .ForEachAsync(async (docSnapshot) =>
                                 {
                                     if (docSnapshot.Exists)
                                         await ImageDatabaseCache.InsertData(docSnapshot.ConvertTo<ImageData>());
                                 }).ConfigureAwait(false);
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

            dbConnection.GetConnection().CreateTable<ImageData>();
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
