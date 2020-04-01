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

        public static async Task<IEnumerable<ImageData>> GetAllImageDataForCondition(String backgroundCode)
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
        }

        public static async Task<ImageData> GetRandomImageForCondition(String backgroundCode)
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
        }

        private static async Task SaveSnapshot(Google.Cloud.Firestore.FirestoreDb firestoreDb)
        {
            await ImageDatabaseCache.ClearCache();

            await firestoreDb.Collection("background_images")
                             .StreamAsync()
                             .ForEachAsync(async (docSnapshot) =>
                             {
                                 if (docSnapshot.Exists)
                                     await ImageDatabaseCache.InsertData(docSnapshot.ConvertTo<ImageData>());
                             }).ConfigureAwait(false);
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

        public async Task<bool> IsEmpty()
        {
            return (await dbConnection.Table<ImageData>().CountAsync()) == 0;
        }

        // Select
        public async Task<IEnumerable<ImageData>> GetAllImageData()
        {
            return await dbConnection.Table<ImageData>().ToListAsync();
        }

        public async Task<IEnumerable<ImageData>> GetAllImageDataForCondition(String backgroundCode)
        {
            return await dbConnection.Table<ImageData>()
                                     .Where(i => Equals(i.Condition, backgroundCode))
                                     .ToListAsync();
        }

        public async Task<ImageData> GetRandomImageForCondition(String backgroundCode)
        {
            // TODO: change if we end up having alot more items
            // This is ok for now as table size will be relatively low
            return await dbConnection.FindWithQueryAsync<ImageData>("SELECT * FROM imagedata WHERE condition = ? ORDER BY RANDOM() LIMIT 1", backgroundCode);
        }

        // Insert
        public async Task InsertData(ImageData imageData)
        {
            await dbConnection.InsertAsync(imageData);
        }

        // Delete
        public async Task ClearCache()
        {
            await dbConnection.DeleteAllAsync<ImageData>();
        }
    }
}
