using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using CommunityToolkit.Mvvm.DependencyInjection;
using Firebase.CloudFirestore;
using Firebase.Database;
using Foundation;
using SimpleWeather.Firebase;
using SimpleWeather.Maui.BackgroundTasks;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData.Images;
using SimpleWeather.WeatherData.Images.Model;

namespace SimpleWeather.Maui.Images;

public static class ImageDatabase
{
    private const string TAG = "ImageDatabase";

    public static async Task<List<ImageData>> GetAllImageDataForCondition(string backgroundCode)
    {
        var imageDataService = Ioc.Default.GetService<IImageDataService>();

        var db = await FirebaseHelper.GetFirestoreDB();
        var query = db.GetCollection("background_images")
            .WhereEqualsTo("condition", backgroundCode);

        QuerySnapshot querySnapshot = null;

        try
        {
            // Try to retrieve from cache first
            if (!imageDataService.ShouldInvalidateCache())
            {
                querySnapshot = await query.GetDocumentsAsync(FirestoreSource.Cache);
            }
        }
        catch
        {
            querySnapshot = null;
        }

        // If data is missing from cache, get data from server
        if (querySnapshot == null)
        {
            try
            {
                querySnapshot = await query.GetDocumentsAsync(FirestoreSource.Server);

                // Run query to cache data
                await SaveSnapshot(db);
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e);
            }
        }

        var list = new List<ImageData>();

        try
        {
            if (querySnapshot != null)
            {
                list.EnsureCapacity(querySnapshot.Documents.Length);
                foreach (var docSnapsnot in querySnapshot.Documents)
                {
                    if (docSnapsnot.Exists)
                    {
                        var dict = docSnapsnot.Data;

                        var imageData = new ImageData().Apply(it =>
                        {
                            it.ArtistName = dict["artistName"]?.ToString();
                            it.HexColor = dict["color"]?.ToString();
                            it.Condition = dict["condition"]?.ToString();
                            it.ImageUrl = dict["imageURL"]?.ToString();
                            it.Location = dict["location"]?.ToString();
                            it.OriginalLink = dict["originalLink"]?.ToString();
                            it.SiteName = dict["siteName"]?.ToString();
                        });

                        if (imageData.ImageUrl != null)
                        {
                            list.Add(imageData);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.WriteLine(LoggerLevel.Error, e);
        }

        return list;
    }

    public static async Task<ImageData> GetRandomImageForCondition(string backgroundCode)
    {
        var imageDataService = Ioc.Default.GetService<IImageDataService>();

        var db = await FirebaseHelper.GetFirestoreDB();
        var query = db.GetCollection("background_images")
            .WhereEqualsTo("condition", backgroundCode);

        QuerySnapshot querySnapshot = null;

        try
        {
            // Try to retrieve from cache first
            if (!imageDataService.ShouldInvalidateCache())
            {
                querySnapshot = await query.GetDocumentsAsync(FirestoreSource.Cache);
            }
        }
        catch
        {
            querySnapshot = null;
        }

        // If data is missing from cache, get data from server
        if (querySnapshot == null || querySnapshot.IsEmpty)
        {
            try
            {
                querySnapshot = await query.GetDocumentsAsync(FirestoreSource.Server);

                // Run query to cache data
                await SaveSnapshot(db);
            }
            catch (Exception e)
            {
                Logger.WriteLine(LoggerLevel.Error, e);
            }
        }

        try
        {
            if (querySnapshot != null)
            {
                var randomDocSnapshot = querySnapshot.Documents.RandomOrDefault();

                if (randomDocSnapshot != null)
                {
                    var dict = randomDocSnapshot.Data;

                    var imageData = new ImageData().Apply(it =>
                    {
                        it.ArtistName = dict["artistName"]?.ToString();
                        it.HexColor = dict["color"]?.ToString();
                        it.Condition = dict["condition"]?.ToString();
                        it.ImageUrl = dict["imageURL"]?.ToString();
                        it.Location = dict["location"]?.ToString();
                        it.OriginalLink = dict["originalLink"]?.ToString();
                        it.SiteName = dict["siteName"]?.ToString();
                    });

                    if (imageData.ImageUrl != null)
                    {
                        return imageData;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.WriteLine(LoggerLevel.Error, e);
        }

        return null;
    }

    private static async Task SaveSnapshot(Firestore db)
    {
        AnalyticsLogger.LogEvent($"{TAG}: saveSnapshot");

        var imageDataService = Ioc.Default.GetService<IImageDataService>();

        // Get all data from server to cache locally
        var query = db.GetCollection("background_images");
        try
        {
            await query.GetDocumentsAsync(FirestoreSource.Server);
            imageDataService.InvalidateCache(false);
            if (imageDataService.GetImageDBUpdateTime() == 0)
            {
                imageDataService.SetImageDBUpdateTime(await GetLastUpdateTime());
            }
        }
        catch (Exception e)
        {
            Logger.WriteLine(LoggerLevel.Error, e);
        }

        // Register worker
        ImageDatabaseTask.CancelPendingTasks();
        ImageDatabaseTask.ScheduleTask();
    }

    public static Task<long> GetLastUpdateTime()
    {
        var tcs = new TaskCompletionSource<long>();

        Task.Run(async () =>
        {
            var db = await FirebaseHelper.GetFirebaseDB();

            var _ref = db.GetRootReference().GetChild("background_images_info")
                .GetChild("collection_info").GetChild("last_updated");

            _ref.ObserveSingleEvent(DataEventType.Value, (snapshot) =>
            {
                var lastUpdated = snapshot.GetValue<NSNumber>();
                tcs.TrySetResult(lastUpdated.Int64Value);
            }, (error) =>
            {
                tcs.TrySetResult(0L);
            });

        });

        return tcs.Task;
    }
}

