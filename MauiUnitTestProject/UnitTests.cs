using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
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
using SimpleWeather.Helpers;
using SimpleWeather.HttpClientExtensions;
using SimpleWeather.LocationData;
using SimpleWeather.RemoteConfig;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API;
using SimpleWeather.Weather_API.AQICN;
using SimpleWeather.Weather_API.HERE;
using SimpleWeather.Weather_API.Json;
using SimpleWeather.Weather_API.NWS;
using SimpleWeather.Weather_API.SMC;
using SimpleWeather.Weather_API.TomorrowIO;
using SimpleWeather.Weather_API.WeatherApi;
using SimpleWeather.WeatherData;
using Xunit;
using Debug = System.Diagnostics.Debug;
using ThreadState = System.Threading.ThreadState;
using WeatherUtils = SimpleWeather.Utils.WeatherUtils;
#if __IOS__
using FirebaseRemoteConfig = Firebase.RemoteConfig.RemoteConfig;

#else
using SimpleWeather.Firebase;
#endif

namespace UnitTestProject
{
    public class UnitTestsFixture
    {
        private static bool Initialized = false;

        public UnitTestsFixture()
        {
            if (!Initialized)
            {
                InitializeDependencies();
                Initialized = true;
            }
        }

        private void InitializeDependencies()
        {
            SharedModule.Instance.Initialize();

            // Add Json Resolvers
            JSONParser.DefaultSettings.AddWeatherAPIContexts();

            // Build DI Services
            SharedModule.Instance.GetServiceCollection().Apply(collection =>
            {
                WeatherModule.Instance.ConfigureServices(collection);
                ExtrasModule.Instance.ConfigureServices(collection);
            });
            SharedModule.Instance.BuildServiceProvider();

            // Initialize post-DI setup; Migrations require rely on DI
            CommonModule.Instance.Initialize();
            ExtrasModule.Instance.Initialize();
        }
    }

    public class UnitTests : IClassFixture<UnitTestsFixture>, IAsyncLifetime
    {
        private static bool WasUsingPersonalKey = false;

        public async Task InitializeAsync()
        {
            SharedModule.Instance.Initialize();

            await Utils.SettingsManager.LoadIfNeeded();

            if (Utils.SettingsManager.UsePersonalKeys[Utils.SettingsManager.API])
            {
                Utils.SettingsManager.UsePersonalKeys[Utils.SettingsManager.API] = false;
                WasUsingPersonalKey = true;
            }
        }

        public Task DisposeAsync()
        {
            if (WasUsingPersonalKey)
            {
                Utils.SettingsManager.UsePersonalKeys[Utils.SettingsManager.API] = true;
                WasUsingPersonalKey = false;
            }

            SharedModule.Instance.Dispose();

            return Task.CompletedTask;
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
            Assert.True(location != null && !location.IsEmpty);
            if (string.IsNullOrWhiteSpace(location?.LocationTZLong) && location.LocationLat != 0 &&
                location.LocationLong != 0)
            {
                string tzId =
                    await WeatherModule.Instance.TZDBService.GetTimeZone(location.LocationLat, location.LocationLong);
                if (!string.IsNullOrWhiteSpace(tzId))
                    location.LocationTZLong = tzId;
            }

            var locData = location.ToLocationData();
            return await provider.GetWeather(locData);
        }

        private async Task<bool> SerializerTest(Weather weather)
        {
            // Serialize weather object
            var serialStr = await JSONParser.SerializerAsync(weather);
            var deserialWeather = await JSONParser.DeserializerAsync<Weather>(serialStr);

            // Serialize forecasts
            {
                var fcast = new Forecasts(weather);
                var serialFcast = await JSONParser.SerializerAsync(fcast);
                var deserialfcast = await JSONParser.DeserializerAsync<Forecasts>(serialFcast);

                deserialWeather.forecast = deserialfcast.forecast;
                deserialWeather.aqi_forecast = deserialfcast.aqi_forecast;
                deserialWeather.min_forecast = deserialfcast.min_forecast;
                deserialWeather.txt_forecast = deserialfcast.txt_forecast;
            }

            // Serialize hourly forecasts
            if (weather.hr_forecast?.Count > 0)
            {
                var hrfcasts = weather.hr_forecast?.Select(hrf => { return new HourlyForecasts(weather.query, hrf); })
                    .ToList();

                var serialHrfcasts = await JSONParser.SerializerAsync(hrfcasts);
                var deserialHrfcasts = await JSONParser.DeserializerAsync<IList<HourlyForecasts>>(serialHrfcasts);
                deserialWeather.hr_forecast = deserialHrfcasts.Select(hrfs => { return hrfs.hr_forecast; }).ToList();
            }

            // Serialize alerts
            if (weather.weather_alerts?.Count > 0)
            {
                var alerts = new WeatherAlerts(weather.query, weather.weather_alerts);

                var serialAlerts = await JSONParser.SerializerAsync(alerts);
                var deserialAlerts = await JSONParser.DeserializerAsync<WeatherAlerts>(serialAlerts);

                deserialWeather.weather_alerts = deserialAlerts.alerts;
            }

            bool testSuccess = Equals(weather, deserialWeather) &&
                               weather?.forecast?.Count == deserialWeather?.forecast?.Count &&
                               weather?.aqi_forecast?.Count == deserialWeather?.aqi_forecast?.Count &&
                               weather?.min_forecast?.Count == deserialWeather?.min_forecast?.Count &&
                               weather?.txt_forecast?.Count == deserialWeather?.txt_forecast?.Count &&
                               weather?.hr_forecast?.Count == deserialWeather?.hr_forecast?.Count;

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
                var customReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(customJson));
                customWeather.FromJson(ref customReader);
                watch1.Stop();
                Debug.WriteLine("Deserialize #{2}: Weather - {0} (Custom): {1}", customWeather.source, watch1.Elapsed,
                    i + 1);
            }

            GC.Collect();
            Task.Delay(5000);

            for (int i = 0; i < SERIALIZER_RUNS; i++)
            {
                var watch2 = Stopwatch.StartNew();
                string utf8Json = JSONParser.Serializer(weather);
                watch2.Stop();
                Debug.WriteLine("Serialize #{2}: Weather - {0} (UTF8JsonGen): {1}", weather.source, watch2.Elapsed,
                    i + 1);

                watch2 = Stopwatch.StartNew();
                var utf8Weather = JSONParser.Deserializer<Weather>(utf8Json);
                watch2.Stop();
                Debug.WriteLine("Deserialize #{2}: Weather - {0} (UTF8JsonGen): {1}", utf8Weather.source,
                    watch2.Elapsed, i + 1);
            }
        }

        [Fact]
        public async Task GetHEREWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Here);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetMetNoWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MetNo);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetNWSAlerts()
        {
            var location = await WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS)
                .GetLocation(new WeatherUtils.Coordinate(47.6721646, -122.1706614)).ConfigureAwait(false);
            var locData = location.ToLocationData();
            var alerts = await new NWSAlertProvider().GetAlerts(locData).ConfigureAwait(false);
            Assert.NotNull(alerts);
        }

        [Fact]
        public async Task GetNWSWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.NWS);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.True(weather?.forecast?.Count > 0 && weather?.hr_forecast?.Count > 0);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetOWMWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetOWMOneCallWeather()
        {
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.OpenWeatherMap] = true;

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.OpenWeatherMap);

            Utils.SettingsManager.APIKeys[WeatherAPI.OpenWeatherMap] = provider.GetAPIKey();

            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));

            Utils.SettingsManager.APIKeys[WeatherAPI.OpenWeatherMap] = null;
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.OpenWeatherMap] = false;
        }

        [Fact]
        public async Task GetWUnlockedWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherUnlocked);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetHEREOAuthToken()
        {
            var token = await Auth.HEREOAuthService.GetBearerToken(true).ConfigureAwait(false);
            Assert.True(!String.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task GetTimeZone()
        {
            var tz = await WeatherModule.Instance.TZDBService.GetTimeZone(0, 0).ConfigureAwait(false);
            Debug.WriteLine("TZTest: tz = " + tz);
            Assert.True(!String.IsNullOrWhiteSpace(tz));
        }

        [Fact]
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
            Assert.NotNull(aqi);
        }

        [Fact]
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
            Assert.True(astro.sunrise != DateTime.MinValue && astro.sunset != DateTime.MinValue);
        }

        [Fact]
        public void UnicodeTest()
        {
            var str = "Siln&#253; morsk&#253; pr&#237;liv o&#269;ak.";
            var uncoded = str.UnescapeUnicode();
            Assert.NotEqual(str, uncoded);
        }

        [Fact]
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

            Assert.True(astro.sunrise != DateTime.MinValue && astro.sunset != DateTime.MinValue &&
                        astro.moonrise != DateTime.MinValue && astro.moonset != DateTime.MinValue);
        }

        [Fact]
        public async Task WeatherAPILocationTest()
        {
            var locationProvider = new WeatherApiLocationProvider();
            var locations = await locationProvider.GetLocations("Redmond, WA", WeatherAPI.WeatherApi)
                .ConfigureAwait(false);
            Assert.True(locations?.Count > 0);

            var queryVM = locations.FirstOrDefault(l => l != null && l.LocationName.StartsWith("Redmond"));
            Assert.NotNull(queryVM);

            var nameModel = await locationProvider.GetLocationFromName(queryVM).ConfigureAwait(false);
            Assert.NotNull(nameModel);
        }

        [Fact]
        public async Task GetMeteoFranceWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.MeteoFrance);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(48.85, 2.34)).ConfigureAwait(false);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetWeatherApiWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherApi);
            var weather = await GetWeather(provider).ConfigureAwait(false);
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetTomorrowIOWeather()
        {
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.TomorrowIo] = true;

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.TomorrowIo);

            Utils.SettingsManager.APIKeys[WeatherAPI.TomorrowIo] = "TomorrowIo_REPLACE_VALUE";

            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(34.0207305, -118.6919157))
                .ConfigureAwait(false); // ~ Los Angeles
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));

            Utils.SettingsManager.APIKeys[WeatherAPI.TomorrowIo] = null;
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.TomorrowIo] = false;
        }

        [Fact]
        public async Task GetWeatherBitIOWeather()
        {
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.WeatherBitIo] = true;

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.WeatherBitIo);

            Utils.SettingsManager.APIKeys[WeatherAPI.WeatherBitIo] = "WeatherBitIo_REPLACE_VALUE";

            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(36.23, -115.25))
                .ConfigureAwait(false); // ~ Nevada
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));

            Utils.SettingsManager.APIKeys[WeatherAPI.WeatherBitIo] = null;
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.WeatherBitIo] = false;
        }

        [Fact]
        public async Task GetWeatherKitWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.Apple);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(48.8589384, 2.264635))
                .ConfigureAwait(false); // ~ Paris, France
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetDWDWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.DWD);
            var weather =
                await GetWeather(provider, new WeatherUtils.Coordinate(52.52, 13.4)).ConfigureAwait(false); // Berlin
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetECCCWeather()
        {
            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.ECCC);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(48.737, -91.984))
                .ConfigureAwait(false); // Banning, ON
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
        }

        [Fact]
        public async Task GetECCCWeather_FR()
        {
            CultureInfo locale = null;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                locale = LocaleUtils.GetLocale();
                LocaleUtils.SetLocaleCode(CultureInfo.GetCultureInfo("fr-CA").Name);
            });

            var provider = WeatherModule.Instance.WeatherManager.GetWeatherProvider(WeatherAPI.ECCC);
            var weather = await GetWeather(provider, new WeatherUtils.Coordinate(48.737, -91.984))
                .ConfigureAwait(false); // Banning, ON
            Assert.True(weather?.IsValid() == true && new WeatherUiModel(weather).IsValid);
            Assert.True(await SerializerTest(weather).ConfigureAwait(false));
            // Restore
            await MainThread.InvokeOnMainThreadAsync(() => { LocaleUtils.SetLocaleCode(locale.Name); });
        }

        /// <summary>
        /// GetPollenData
        /// </summary>
        /// <returns></returns>
        /// <exception cref="WeatherException">Ignore.</exception>
        [Fact]
        public async Task GetPollenData()
        {
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.TomorrowIo] = true;
            Utils.SettingsManager.APIKeys[WeatherAPI.TomorrowIo] = "TomorrowIo_REPLACE_VALUE";

            var provider = new TomorrowIOWeatherProvider();
            var location = await provider.GetLocation(new WeatherUtils.Coordinate(34.0207305, -118.6919157))
                .ConfigureAwait(false); // ~ Los Angeles
            var locData = location.ToLocationData();
            Assert.NotNull(locData);
            var pollenData = await provider.GetPollenData(locData);
            Assert.NotNull(pollenData);

            Utils.SettingsManager.APIKeys[WeatherAPI.TomorrowIo] = null;
            Utils.SettingsManager.UsePersonalKeys[WeatherAPI.TomorrowIo] = false;
        }

        [Fact]
        public async Task RemoteConfigUpdateTest()
        {
#if WINDOWS
            var remoteConfig = await FirebaseHelper.GetFirebaseRemoteConfig();
            var response = await remoteConfig.GetRemoteConfig();
            Assert.True(response.entries.Count > 0);
#else
#if __IOS__
            var remoteConfig = FirebaseRemoteConfig.SharedInstance;
            await remoteConfig.FetchAndActivateAsync();
            var configJson = remoteConfig.GetConfigValue(WeatherAPI.WeatherApi)?.StringValue;
            var config = JSONParser.Deserializer<WeatherProviderConfig>(configJson);
            Assert.NotNull(config);
#else
            var db = await FirebaseHelper.GetFirebaseDatabase();
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
                        Assert.NotNull(config);
                    }
                }
            }
#endif
#endif
        }

        [Fact]
        public async Task CacheCowTest()
        {
            var CacheRootDir = Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "CacheCowTest");
            if (Directory.Exists(CacheRootDir))
            {
                Directory.Delete(CacheRootDir, true);
            }

            var client =
                ClientExtensions.CreateClient(new RemoveHeaderDelagatingCacheStore(new FileStore(CacheRootDir)));
            const string CacheableResource = "https://code.jquery.com/jquery-3.3.1.slim.min.js";
            var response = await client.GetAsync(CacheableResource);
            var responseFromCache = await client.GetAsync(CacheableResource);

            var serverResponseHeaders = response.Headers.GetCacheCowHeader();
            Assert.False(serverResponseHeaders.RetrievedFromCache.GetValueOrDefault(false));

            var cacheResponseHeaders = responseFromCache.Headers.GetCacheCowHeader();
            Assert.True(cacheResponseHeaders.RetrievedFromCache.GetValueOrDefault(false));
        }

        [Fact]
        public async Task ImageHeaderTest()
        {
            var results = new List<ImageUtils.ImageType>();

            {
                var ImageCacheFolder =
                    new DirectoryInfo(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "images"));
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
                var AssetsFolder = new DirectoryInfo(Path.Combine(AppFolder, "SimpleWeather.Shared", "Resources",
                    "Images", "Backgrounds"));
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

            Assert.DoesNotContain(ImageUtils.ImageType.Unknown, results);
        }

        [Fact]
        public async Task ImageFileTest()
        {
            var ImageCacheFolder =
                new DirectoryInfo(Path.Combine(ApplicationDataHelper.GetLocalCacheFolderPath(), "images"));
            if (!ImageCacheFolder.Exists)
            {
                ImageCacheFolder.Create();
            }

            var CacheFiles = ImageCacheFolder.GetFiles();
            var file = CacheFiles.First();

            var t1 = new Thread(() =>
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
                        t1.ThreadState == ThreadState.Stopped &&
                        t2.ThreadState == ThreadState.Stopped &&
                        t3.ThreadState == ThreadState.Stopped &&
                        t4.ThreadState == ThreadState.Stopped &&
                        t5.ThreadState == ThreadState.Stopped;
                } while (!completed);
            });
        }

        [Fact]
        public async Task LocalImageTest()
        {
            var repo = new ImageDataHelperRes();

            var data = await repo.GetRemoteImageData(WeatherBackground.DAY);

            Assert.True(data != null && await data.IsImageValid());
        }
    }
}