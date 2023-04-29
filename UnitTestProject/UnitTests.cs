using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using CacheCow.Client.Headers;
using SimpleWeather;
using SimpleWeather.Backgrounds;
using SimpleWeather.Common;
using SimpleWeather.Common.Controls;
using SimpleWeather.Common.Images;
using SimpleWeather.DI;
using SimpleWeather.Extras;
using SimpleWeather.Firebase;
using SimpleWeather.Helpers;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.LocationData;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.HERE;
using SimpleWeather.Weather_API.NWS;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.TomorrowIO;
using SimpleWeather.Weather_API.WeatherApi;
using SimpleWeather.WeatherData;
using System.Diagnostics;
using System.Text;
using WeatherUtils = SimpleWeather.Utils.WeatherUtils;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        private static bool WasUsingPersonalKey = false;

        [ClassInitialize]
        public static async Task Initialize(TestContext _)
        {
            InitializeDependencies();

            await Utils.SettingsManager.LoadIfNeeded();

            if (Utils.SettingsManager.UsePersonalKey)
            {
                Utils.SettingsManager.UsePersonalKey = false;
                WasUsingPersonalKey = true;
            }
        }

        [ClassCleanup]
        public static void Destroy()
        {
            if (WasUsingPersonalKey)
            {
                Utils.SettingsManager.UsePersonalKey = true;
                WasUsingPersonalKey = false;
            }
        }

        private static void InitializeDependencies()
        {
            SharedModule.Instance.Initialize();

            // Set UTF8Json Resolver
            Utf8Json.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
                JSONParser.Resolver,
                SimpleWeather.Weather_API.Utf8JsonGen.Resolvers.GeneratedResolver.Instance
            );

            CommonModule.Instance.Initialize();
            ExtrasModule.Instance.Initialize();

            // Build DI Services
            SharedModule.Instance.GetServiceCollection().Apply(collection =>
            {
                WeatherModule.Instance.ConfigureServices(collection);
                ExtrasModule.Instance.ConfigureServices(collection);
            });
            SharedModule.Instance.BuildServiceProvider();
        }

        private Task<Weather> GetWeather(IWeatherProvider provider)
        {
            /* Redmond, WA */
            return GetWeather(provider, new WeatherUtils.Coordinate(47.6721646, -122.1706614));
        }

        /// <summary>
        /// GetWeather
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task<Weather> GetWeather(IWeatherProvider provider, WeatherUtils.Coordinate coordinate)
        {
            var location = await provider.GetLocation(coordinate);
            Assert.IsNotNull(location);
            if (string.IsNullOrWhiteSpace(location?.LocationTZLong) && location.LocationLat != 0 && location.LocationLong != 0)
            {
                string tzId = await WeatherModule.Instance.TZDBService.GetTimeZone(location.LocationLat, location.LocationLong);
                if (!string.IsNullOrWhiteSpace(tzId))
                    location.LocationTZLong = tzId;
            }
            var locData = location.ToLocationData();
            return await provider.GetWeather(locData);
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
        public async Task GetHEREWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Here);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetMetNoWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MetNo);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetNWSAlerts()
        {
            var location = await WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS)
                .GetLocation(new WeatherUtils.Coordinate(47.6721646, -122.1706614)).ConfigureAwait(false);
            var locData = location.ToLocationData();
            var alerts = await new NWSAlertProvider().GetAlerts(locData).ConfigureAwait(false);
            Assert.IsNotNull(alerts);
        }

        [TestMethod]
        public async Task GetNWSWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.forecast?.Count > 0 && weather?.hr_forecast?.Count > 0);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetOWMWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetOWMOneCallWeather()
        {
            Utils.SettingsManager.UsePersonalKey = true;

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap);

            Utils.SettingsManager.APIKeys[WeatherAPI.OpenWeatherMap] = provider.GetAPIKey();

            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));

            Utils.SettingsManager.APIKeys[WeatherAPI.OpenWeatherMap] = null;
            Utils.SettingsManager.UsePersonalKey = false;
        }

        [TestMethod]
        public async Task GetWUnlockedWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherUnlocked);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetHEREOAuthToken()
        {
            var token = await Auth.HEREOAuthService.GetBearerToken(true).ConfigureAwait(false);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public async Task GetTimeZone()
        {
            var tz = await WeatherModule.Instance.TZDBService.GetTimeZone(0, 0).ConfigureAwait(false);
            Debug.WriteLine("TZTest: tz = " + tz);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(tz));
        }

        [TestMethod]
        public async Task GetAQIData()
        {
            var tz_long = "America/Los_Angeles";
            var aqi = await new SimpleWeather.Weather_API.AQICN.AQICNProvider().GetAirQualityData(
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
            var date = DateTimeOffset.Now.AddDays(-1.5);
            var tz_long = "America/Anchorage";
            var locationData = new LocationData()
            {
                latitude = 71.17,
                longitude = -156.47,
                tz_long = tz_long
            };
            var astro = await new SunMoonCalcProvider().GetAstronomyData(locationData, date).ConfigureAwait(false);

            Debug.WriteLine("SMC");
            Debug.WriteLine(String.Format(
                "Sunrise: {0}; Sunset: {1}; Moonrise: {2}; Moonset: {3}",
                astro.sunrise, astro.sunset, astro.moonrise, astro.moonset));
            if (astro.moonphase != null)
            {
                Debug.WriteLine(String.Format("Moonphase: {0}", astro.moonphase.phase));
            }

            Assert.IsTrue(astro.sunrise != DateTime.MinValue && astro.sunset != DateTime.MinValue && astro.moonrise != DateTime.MinValue && astro.moonset != DateTime.MinValue);
        }

        [TestMethod]
        public async Task WeatherAPILocationTest()
        {
            var locationProvider = new WeatherApiLocationProvider();
            var locations = await locationProvider.GetLocations("Redmond, WA", WeatherAPI.WeatherApi).ConfigureAwait(false);
            Assert.IsTrue(locations?.Count > 0);

            var queryVM = locations.FirstOrDefault(l => l != null && l.LocationName.StartsWith("Redmond"));
            Assert.IsNotNull(queryVM);

            var nameModel = await locationProvider.GetLocationFromName(queryVM).ConfigureAwait(false);
            Assert.IsNotNull(nameModel);
        }

        [TestMethod]
        public async Task GetMeteoFranceWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(48.85, 2.34)).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetWeatherApiWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherApi);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetTomorrowIOWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.TomorrowIo);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(34.0207305, -118.6919157)).ConfigureAwait(false); // ~ Los Angeles
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetWeatherBitIOWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherBitIo);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(36.23, -115.25)).ConfigureAwait(false); // ~ Nevada
            Assert.IsTrue(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.IsTrue(await SerializerTest(weather).ConfigureAwait(false));
        }

        /// <summary>
        /// GetPollenData
        /// </summary>
        /// <returns></returns>
        /// <exception cref="WeatherException">Ignore.</exception>
        [TestMethod]
        public async Task GetPollenData()
        {
            var provider = new TomorrowIOWeatherProvider();
            var location = await provider.GetLocation(new WeatherUtils.Coordinate(34.0207305, -118.6919157)).ConfigureAwait(false); // ~ Los Angeles
            var locData = location.ToLocationData();
            Assert.IsNotNull(locData);
            var pollenData = await provider.GetPollenData(locData);
            Assert.IsNotNull(pollenData);
        }

        [TestMethod]
        public async Task RemoteConfigUpdateTest()
        {
            var db = await FirebaseDatabaseHelper.GetFirebaseDatabase();
#if __IOS__
            var uwpConfig = await db.Child("ios_remote_config").OnceAsync<object>();
#else
            var uwpConfig = await db.Child("uwp_remote_config").OnceAsync<object>();
#endif
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
            var CacheRootDir = Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "CacheCowTest");
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
                var ImageCacheFolder = new DirectoryInfo(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "images"));
                if (!ImageCacheFolder.Exists)
                {
                    ImageCacheFolder.Create();
                }
                var CacheFiles = ImageCacheFolder.GetFiles();

                foreach (var file in CacheFiles)
                {
                    while (FileUtils.IsFileLocked(file.FullName))
                    {
                        await Task.Delay(100);
                    }

                    using var stream = new BufferedStream(file.OpenRead());
                    var imageType = ImageUtils.GuessImageType(stream);
                    Debug.WriteLine($"ImageTest: file path: {file.FullName}");
                    Debug.WriteLine($"ImageTest: type: {imageType}");
                    results.Add(imageType);
                    //Assert.AreNotEqual(imageType, ImageUtils.ImageType.Unknown);
                }
            }

            {
                var AppFolder = AppDomain.CurrentDomain.BaseDirectory;
                var AssetsFolder = new DirectoryInfo(Path.Combine(AppFolder, "SimpleWeather.Shared", "Assets", "Backgrounds"));
                var AssetFiles = AssetsFolder.GetFiles();

                foreach (var file in AssetFiles)
                {
                    while (FileUtils.IsFileLocked(file.FullName))
                    {
                        await Task.Delay(100);
                    }

                    using var stream = new BufferedStream(file.OpenRead());
                    var imageType = ImageUtils.GuessImageType(stream);
                    Debug.WriteLine($"ImageTest: file path: {file.FullName}");
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
            var ImageCacheFolder = new DirectoryInfo(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "images"));
            if (!ImageCacheFolder.Exists)
            {
                ImageCacheFolder.Create();
            }
            var CacheFiles = ImageCacheFolder.GetFiles();
            var file = CacheFiles.First();

            var t1 = new Thread(async () =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file.FullName))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(file.OpenRead());
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.FullName}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t2 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file.FullName))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(file.OpenRead());
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.FullName}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t3 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file.FullName))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(file.OpenRead());
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.FullName}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t4 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file.FullName))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(file.OpenRead());
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.FullName}");
                Debug.WriteLine($"ImageTest: type: {imageType}");
            });

            var t5 = new Thread(() =>
            {
                Thread.Sleep(2000);
                while (FileUtils.IsFileLocked(file.FullName))
                {
                    Thread.Sleep(100);
                }

                using var stream = new BufferedStream(file.OpenRead());
                var imageType = ImageUtils.GuessImageType(stream);
                Debug.WriteLine($"ImageTest: file path: {file.FullName}");
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