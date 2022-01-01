using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using CacheCow.Client.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWeather.AQICN;
using SimpleWeather.Backgrounds;
using SimpleWeather.Controls;
using SimpleWeather.Firebase;
using SimpleWeather.HERE;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.Location;
using SimpleWeather.NWS;
using SimpleWeather.RemoteConfig;
using SimpleWeather.SMC;
using SimpleWeather.TZDB;
using SimpleWeather.Utils;
using SimpleWeather.WeatherApi;
using SimpleWeather.WeatherData;
using SimpleWeather.WeatherData.Images;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        private bool WasUsingPersonalKey = false;

        [TestInitialize]
        public async Task Initialize()
        {
            await Settings.LoadIfNeededAsync();

            if (Settings.UsePersonalKey)
            {
                Settings.UsePersonalKey = false;
                WasUsingPersonalKey = true;
            }
        }

        [TestCleanup]
        public void Destroy()
        {
            if (WasUsingPersonalKey)
            {
                Settings.UsePersonalKey = true;
                WasUsingPersonalKey = false;
            }
        }

        private Task<Weather> GetWeather(WeatherProviderImpl providerImpl)
        {
            /* Redmond, WA */
            return GetWeather(providerImpl, new WeatherUtils.Coordinate(47.6721646, -122.1706614));
        }

        /// <summary>
        /// GetWeather
        /// </summary>
        /// <param name="providerImpl"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task<Weather> GetWeather(WeatherProviderImpl providerImpl, WeatherUtils.Coordinate coordinate)
        {
            var location = await providerImpl.GetLocation(coordinate);
            Assert.IsNotNull(location);
            if (string.IsNullOrWhiteSpace(location?.LocationTZLong) && location.LocationLat != 0 && location.LocationLong != 0)
            {
                string tzId = await TZDBCache.GetTimeZone(location.LocationLat, location.LocationLong);
                if (!string.IsNullOrWhiteSpace(tzId))
                    location.LocationTZLong = tzId;
            }
            var locData = new LocationData(location);
            return await providerImpl.GetWeather(locData);
        }

        private async Task<bool> SerializerTest(Weather weather)
        {
            var serialStr = await JSONParser.SerializerAsync(weather);
            var deserialWeather = await JSONParser.DeserializerAsync<Weather>(serialStr);
            var fcast = new Forecasts(weather);
            var serialFcast = await JSONParser.SerializerAsync(fcast);
            var deserialfcast = await JSONParser.DeserializerAsync<Forecasts>(serialFcast);
            bool testSuccess = Equals(weather, deserialWeather) && string.Equals(fcast?.query, deserialfcast?.query) &&
                fcast?.forecast?.Count == deserialfcast?.forecast?.Count && fcast?.txt_forecast?.Count == deserialfcast?.txt_forecast?.Count;
            if (weather.hr_forecast?.Count > 0)
            {
                var hfcast = new HourlyForecasts(weather.query, weather.hr_forecast?[0]);
                var serialHr = await JSONParser.SerializerAsync(hfcast);
                var deserialHr = await JSONParser.DeserializerAsync<HourlyForecasts>(serialHr);
                testSuccess = testSuccess && string.Equals(fcast?.query, deserialfcast?.query) &&
                    Equals(hfcast?.hr_forecast, deserialHr?.hr_forecast) && hfcast?.date == deserialHr?.date;
            }
            return testSuccess;
        }

        private const int SERIALIZER_RUNS = 5;

        private void SerializerSpeedTest(Weather weather)
        {
            for (int i = 0; i < SERIALIZER_RUNS; i++)
            {
                var watch1 = Stopwatch.StartNew();
                string customJson = weather.ToJson();
                watch1.Stop();
                Debug.WriteLine("Serialize #{2}: Weather - {0} (Custom): {1}", weather.source, watch1.Elapsed, i + 1);

                watch1 = Stopwatch.StartNew();
                var customWeather = new Weather();
                var customReader = new Utf8Json.JsonReader(Encoding.UTF8.GetBytes(customJson));
                customWeather.FromJson(ref customReader);
                watch1.Stop();
                Debug.WriteLine("Deserialize #{2}: Weather - {0} (Custom): {1}", customWeather.source, watch1.Elapsed, i + 1);
            }

            GC.Collect();
            Task.Delay(5000);

            for (int i = 0; i < SERIALIZER_RUNS; i++)
            {
                var watch2 = Stopwatch.StartNew();
                string utf8Json = JSONParser.Serializer(weather);
                watch2.Stop();
                Debug.WriteLine("Serialize #{2}: Weather - {0} (UTF8JsonGen): {1}", weather.source, watch2.Elapsed, i + 1);

                watch2 = Stopwatch.StartNew();
                var utf8Weather = JSONParser.Deserializer<Weather>(utf8Json);
                watch2.Stop();
                Debug.WriteLine("Deserialize #{2}: Weather - {0} (UTF8JsonGen): {1}", utf8Weather.source, watch2.Elapsed, i + 1);
            }
        }

        [TestMethod]
        public async Task GetHereWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.Here);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetMetNoWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MetNo);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetNWSAlerts()
        {
            var location = await WeatherManager.GetProvider(WeatherAPI.NWS)
                .GetLocation(new WeatherUtils.Coordinate(47.6721646, -122.1706614)).ConfigureAwait(false);
            var locData = new LocationData(location);
            var alerts = await new NWSAlertProvider().GetAlerts(locData).ConfigureAwait(false);
            Assert.IsNotNull(alerts);
        }

        [TestMethod]
        public async Task GetNWSWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.forecast?.Count > 0 && weather?.hr_forecast?.Count > 0);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetOWMWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetOWMOneCallWeather()
        {
            Settings.UsePersonalKey = true;

            var provider = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);

            Settings.API_KEY = provider.GetAPIKey();

            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));

            Settings.API_KEY = null;
            Settings.UsePersonalKey = false;
        }

        [TestMethod]
        public async Task GetWUnlockedWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.WeatherUnlocked);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetHEREOAuthToken()
        {
            var token = await HEREOAuthUtils.GetBearerToken(true).ConfigureAwait(false);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public async Task GetTimeZone()
        {
            var tz = await new TimeZoneProvider().GetTimeZone(0, 0).ConfigureAwait(false);
            Debug.WriteLine("TZTest: tz = " + tz);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(tz));
        }

        [TestMethod]
        public async Task GetAQIData()
        {
            var tz_long = "America/Los_Angeles";
            var aqi = await new AQICNProvider().GetAirQualityData(
                new LocationData()
                {
                    latitude = 47.6721646,
                    longitude = -122.1706614,
                    tz_long = tz_long
                }).ConfigureAwait(false);
            Assert.IsNotNull(aqi);
        }

        [TestMethod]
        public async Task GetSunriseSetTime()
        {
            var date = DateTimeOffset.UtcNow;
            var tz_long = "America/Los_Angeles";
            var astro = await new SolCalcAstroProvider().GetAstronomyData(
                new LocationData()
                {
                    latitude = 47.6721646,
                    longitude = -122.1706614,
                    tz_long = tz_long
                }, date).ConfigureAwait(false);
            Debug.WriteLine("SolCalc - Sunrise: {0}; Sunset: {1}", astro.sunrise, astro.sunset);
            Assert.IsTrue(astro.sunrise != DateTime.MinValue && astro.sunset != DateTime.MinValue);
        }

        [TestMethod]
        public void UnicodeTest()
        {
            var str = "Siln&#253; morsk&#253; pr&#237;liv o&#269;ak.";
            var uncoded = str.UnescapeUnicode();
            Assert.AreNotEqual(str, uncoded);
        }

        [TestMethod]
        public async Task SimpleAstroTest()
        {
            var date = DateTimeOffset.Now;
            var tz_long = "America/Los_Angeles";
            var locationData = new LocationData()
            {
                latitude = 47.6721646,
                longitude = -122.1706614,
                tz_long = tz_long
            };
            var astro = await new SunMoonCalcProvider().GetAstronomyData(locationData, date).ConfigureAwait(false);

            Console.WriteLine("SMC");
            Console.WriteLine(String.Format(
                "Sunrise: {0}; Sunset: {1}; Moonrise: {2}; Moonset: {3}",
                astro.sunrise, astro.sunset, astro.moonrise, astro.moonset));
            if (astro.moonphase != null)
            {
                Console.WriteLine(String.Format("Moonphase: {0}", astro.moonphase.phase));
            }

            Assert.IsTrue(astro.sunrise != DateTime.MinValue && astro.sunset != DateTime.MinValue && astro.moonrise != DateTime.MinValue && astro.moonset != DateTime.MinValue);
        }

        [TestMethod]
        public async Task WeatherAPILocationTest()
        {
            var locationProvider = new WeatherApiLocationProvider();
            var locations = await locationProvider.GetLocations("Redmond, WA", WeatherAPI.WeatherApi).ConfigureAwait(false);
            Assert.IsTrue(locations?.Count > 0);

            var queryVM = locations.FirstOrDefault(l => l != null && l.LocationName.StartsWith("Redmond, "));
            Assert.IsNotNull(queryVM);

            var nameModel = await locationProvider.GetLocationFromName(queryVM).ConfigureAwait(false);
            Assert.IsNotNull(nameModel);
        }

        [TestMethod]
        public async Task GetMeteoFranceWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MeteoFrance);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(48.85, 2.34)).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetWeatherApiWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.WeatherApi);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetTomorrowIOWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.TomorrowIo);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(34.0207305, -118.6919157)).ConfigureAwait(false); // ~ Los Angeles
            Assert.IsTrue(weather?.IsValid() == true && new WeatherNowViewModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task RemoteConfigUpdateTest()
        {
            var db = await FirebaseDatabaseHelper.GetFirebaseDatabase();
            var uwpConfig = await db.Child("uwp_remote_config").OnceAsync<object>();
            if (uwpConfig?.Count > 0)
            {
                foreach (var prop in uwpConfig)
                {
                    if (!Equals(prop.Key, "default_weather_provider"))
                    {
                        Debug.WriteLine("RemoteConfigUpdateTest: KEY = {0}", (object)prop.Key);
                        var configJson = prop.Object.ToString();
                        var config = JSONParser.Deserializer<WeatherProviderConfig>(configJson);
                        Assert.IsNotNull(config);
                    }
                }
            }
        }

        [TestMethod]
        public async Task CacheCowTest()
        {
            var CacheRootDir = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "CacheCowTest");
            if (Directory.Exists(CacheRootDir))
            {
                Directory.Delete(CacheRootDir, true);
            }
            var client = ClientExtensions.CreateClient(new RemoveHeaderDelagatingCacheStore(new FileStore(CacheRootDir)));
            const string CacheableResource = "https://code.jquery.com/jquery-3.3.1.slim.min.js";
            var response = await client.GetAsync(CacheableResource);
            var responseFromCache = await client.GetAsync(CacheableResource);

            var serverResponseHeaders = response.Headers.GetCacheCowHeader();
            Assert.IsFalse(serverResponseHeaders.RetrievedFromCache.GetValueOrDefault(false));

            var cacheResponseHeaders = responseFromCache.Headers.GetCacheCowHeader();
            Assert.IsTrue(cacheResponseHeaders.RetrievedFromCache.GetValueOrDefault(false));
        }

        [TestMethod]
        public async Task ImageHeaderTest()
        {
            var results = new List<ImageUtils.ImageType>();

            {
                var CacheRootDir = ApplicationData.Current.LocalCacheFolder;
                var CacheFolder = await CacheRootDir.GetFolderAsync("images");
                var CacheFiles = await CacheFolder.GetFilesAsync();

                foreach (var file in CacheFiles)
                {
                    while (FileUtils.IsFileLocked(file))
                    {
                        await Task.Delay(100);
                    }

                    using var stream = new BufferedStream(await file.OpenStreamForReadAsync());
                    var imageType = ImageUtils.GuessImageType(stream);
                    Debug.WriteLine($"ImageTest: file path: {file.Path}");
                    Debug.WriteLine($"ImageTest: type: {imageType}");
                    results.Add(imageType);
                    //Assert.AreNotEqual(imageType, ImageUtils.ImageType.Unknown);
                }
            }

            {
                var AppFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var AssetsFolder = await (await (await AppFolder.GetFolderAsync("SimpleWeather.Shared")).GetFolderAsync("Assets")).GetFolderAsync("Backgrounds");
                var AssetFiles = await AssetsFolder.GetFilesAsync();

                foreach (var file in AssetFiles)
                {
                    while (FileUtils.IsFileLocked(file))
                    {
                        await Task.Delay(100);
                    }

                    using var stream = new BufferedStream(await file.OpenStreamForReadAsync());
                    var imageType = ImageUtils.GuessImageType(stream);
                    Debug.WriteLine($"ImageTest: file path: {file.Path}");
                    Debug.WriteLine($"ImageTest: type: {imageType}");
                    results.Add(imageType);
                    //Assert.AreNotEqual(imageType, ImageUtils.ImageType.Unknown);
                }
            }

            Assert.IsFalse(results.Contains(ImageUtils.ImageType.Unknown));
        }

        [TestMethod]
        public async Task ImageFileTest()
        {
            var CacheRootDir = ApplicationData.Current.LocalCacheFolder;
            var CacheFolder = await CacheRootDir.GetFolderAsync("images");
            var CacheFiles = await CacheFolder.GetFilesAsync();
            var file = CacheFiles.First();

            var t1 = new Thread(async () =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(await file.OpenStreamForReadAsync());
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.Path}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t2 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(File.OpenRead(file.Path));
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.Path}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t3 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(File.OpenRead(file.Path));
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.Path}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t4 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(File.OpenRead(file.Path));
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.Path}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t5 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(File.Open(file.Path, FileMode.Open, FileAccess.Read, FileShare.Read));
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.Path}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();

            await Task.Run(() =>
            {
                bool completed = false;

                do
                {
                    completed =
                    t1.ThreadState == System.Threading.ThreadState.Stopped &&
                    t2.ThreadState == System.Threading.ThreadState.Stopped &&
                    t3.ThreadState == System.Threading.ThreadState.Stopped &&
                    t4.ThreadState == System.Threading.ThreadState.Stopped &&
                    t5.ThreadState == System.Threading.ThreadState.Stopped;
                }
                while (!completed);
            });
        }

        [TestMethod]
        public async Task LocalImageTest()
        {
            var repo = new ImageDataHelperRes();

            var data = await repo.GetRemoteImageData(WeatherBackground.DAY);

            Assert.IsTrue(data != null && await data.IsImageValid());
        }
    }
}