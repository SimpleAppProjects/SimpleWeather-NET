using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWeather.AQICN;
using SimpleWeather.Firebase;
using SimpleWeather.HERE;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.NWS;
using SimpleWeather.SMC;
using SimpleWeather.TZDB;
using SimpleWeather.Utils;
using SimpleWeather.WeatherApi;
using SimpleWeather.WeatherData;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Settings.LoadIfNeeded();
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
            var locData = new LocationData(location);
            return await providerImpl.GetWeather(locData);
        }

        private async Task<bool> SerializerTest(Weather weather)
        {
            var serialStr = await JSONParser.SerializerAsync(weather);
            var deserialWeather = await JSONParser.DeserializerAsync<Weather>(serialStr);
            var fcast = new Forecasts(weather.query, weather.forecast, weather.txt_forecast);
            var serialFcast = await JSONParser.SerializerAsync(fcast);
            var deserialfcast = await JSONParser.DeserializerAsync<Forecasts>(serialFcast);
            bool testSuccess = Object.Equals(weather, deserialWeather) && string.Equals(fcast?.query, deserialfcast?.query) &&
                fcast?.forecast?.Count == deserialfcast?.forecast?.Count && fcast?.txt_forecast?.Count == deserialfcast?.txt_forecast?.Count;
            if (weather.hr_forecast?.Count > 0)
            {
                var hfcast = new HourlyForecasts(weather.query, weather.hr_forecast?[0]);
                var serialHr = await JSONParser.SerializerAsync(hfcast);
                var deserialHr = await JSONParser.DeserializerAsync<HourlyForecasts>(serialHr);
                testSuccess = testSuccess && string.Equals(fcast?.query, deserialfcast?.query) &&
                    Object.Equals(hfcast?.hr_forecast, deserialHr?.hr_forecast) && hfcast?.date == deserialHr?.date;
            }
            return testSuccess;
        }

        private const int SERIALIZER_RUNS = 5;

        private void SerializerSpeedTest(Weather weather)
        {
            for (int i = 0; i < SERIALIZER_RUNS; i++)
            {
                var watch1 = System.Diagnostics.Stopwatch.StartNew();
                string customJson = weather.ToJson();
                watch1.Stop();
                System.Diagnostics.Debug.WriteLine("Serialize #{2}: Weather - {0} (Custom): {1}", weather.source, watch1.Elapsed, i + 1);

                watch1 = System.Diagnostics.Stopwatch.StartNew();
                var customWeather = new Weather();
                var customReader = new Utf8Json.JsonReader(System.Text.Encoding.UTF8.GetBytes(customJson));
                customWeather.FromJson(ref customReader);
                watch1.Stop();
                System.Diagnostics.Debug.WriteLine("Deserialize #{2}: Weather - {0} (Custom): {1}", customWeather.source, watch1.Elapsed, i + 1);
            }

            System.GC.Collect();
            Task.Delay(5000);

            for (int i = 0; i < SERIALIZER_RUNS; i++)
            {
                var watch2 = System.Diagnostics.Stopwatch.StartNew();
                string utf8Json = JSONParser.Serializer(weather);
                watch2.Stop();
                System.Diagnostics.Debug.WriteLine("Serialize #{2}: Weather - {0} (UTF8JsonGen): {1}", weather.source, watch2.Elapsed, i + 1);

                watch2 = System.Diagnostics.Stopwatch.StartNew();
                var utf8Weather = JSONParser.Deserializer<Weather>(utf8Json);
                watch2.Stop();
                System.Diagnostics.Debug.WriteLine("Deserialize #{2}: Weather - {0} (UTF8JsonGen): {1}", utf8Weather.source, watch2.Elapsed, i + 1);
            }
        }

        [TestMethod]
        public void GetHereWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.Here);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
            //SerializerSpeedTest(weather);
        }

        [TestMethod]
        public void GetMetNoWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MetNo);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
            //SerializerSpeedTest(weather);
        }

        [TestMethod]
        public void GetNWSAlerts()
        {
            var location = WeatherManager.GetProvider(WeatherAPI.NWS)
                .GetLocation(new WeatherUtils.Coordinate(47.6721646, -122.1706614)).ConfigureAwait(false).GetAwaiter().GetResult();
            var locData = new LocationData(location);
            var alerts = new NWSAlertProvider().GetAlerts(locData).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(alerts);
        }

        [TestMethod]
        public void GetNWSWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.forecast?.Count > 0 && weather?.hr_forecast?.Count > 0);
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
            //SerializerSpeedTest(weather);
        }

        [TestMethod]
        public void GetOWMWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
            //SerializerSpeedTest(weather);
        }

        [TestMethod]
        public void GetOWMOneCallWeather()
        {
            Settings.UsePersonalKey = true;

            var provider = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);

            Settings.API_KEY = provider.GetAPIKey();

            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
            //SerializerSpeedTest(weather);

            Settings.API_KEY = null;
            Settings.UsePersonalKey = false;
        }

        [TestMethod]
        public void GetWUnlockedWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.WeatherUnlocked);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
            //SerializerSpeedTest(weather);
        }

        [TestMethod]
        public void GetHEREOAuthToken()
        {
            var token = HEREOAuthUtils.GetBearerToken(true).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(!String.IsNullOrWhiteSpace(token));
        }

        [TestMethod]
        public void GetTimeZone()
        {
            var tz = new TimeZoneProvider().GetTimeZone(0, 0).ConfigureAwait(false).GetAwaiter().GetResult();
            Debug.WriteLine("TZTest: tz = " + tz);
            Assert.IsTrue(!String.IsNullOrWhiteSpace(tz));
        }

        [TestMethod]
        public void GetAQIData()
        {
            var tz_long = "America/Los_Angeles";
            var aqi = new AQICNProvider().GetAirQualityData(
                new LocationData()
                {
                    latitude = 47.6721646,
                    longitude = -122.1706614,
                    tz_long = tz_long
                }).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(aqi);
        }

        [TestMethod]
        public void GetSunriseSetTime()
        {
            var date = DateTimeOffset.UtcNow;
            var tz_long = "America/Los_Angeles";
            var astro = new SolCalcAstroProvider().GetAstronomyData(
                new LocationData()
                {
                    latitude = 47.6721646,
                    longitude = -122.1706614,
                    tz_long = tz_long
                }, date).ConfigureAwait(false).GetAwaiter().GetResult();
            Debug.WriteLine("SolCalc - Sunrise: {0}; Sunset: {1}", astro.sunrise, astro.sunset);
            Assert.IsTrue(astro.sunrise != DateTime.MinValue && astro.sunset != DateTime.MinValue);
        }

        [TestMethod]
        public void RealtimeDBTest()
        {
            var updateTime = SimpleWeather.WeatherData.Images.ImageDatabase.GetLastUpdateTime().ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(updateTime > 0);
        }

        [TestMethod]
        public void FirestoreAPITest()
        {
            var storage = SimpleWeather.Firebase.FirebaseStorageHelper.GetFirebaseStorage().ConfigureAwait(false).GetAwaiter().GetResult();
            var storageRef = storage.GetReferenceFromUrl(new Uri(""));
        }

        [TestMethod]
        public void UnicodeTest()
        {
            var str = "Siln&#253; morsk&#253; pr&#237;liv o&#269;ak.";
            var uncoded = str.UnescapeUnicode();
            Assert.AreNotEqual(str, uncoded);
        }

        [TestMethod]
        public void SimpleAstroTest()
        {
            var date = DateTimeOffset.Now;
            var tz_long = "America/Los_Angeles";
            var locationData = new LocationData()
            {
                latitude = 47.6721646,
                longitude = -122.1706614,
                tz_long = tz_long
            };
            SimpleWeather.WeatherData.Astronomy astro = new SunMoonCalcProvider().GetAstronomyData(locationData, date).ConfigureAwait(false).GetAwaiter().GetResult();

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
        public void WeatherAPILocationTest()
        {
            var locationProvider = new WeatherApiLocationProvider();
            var locations = locationProvider.GetLocations("Redmond, WA", null).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(locations?.Count > 0);

            var queryVM = locations.FirstOrDefault(l => l != null && l.LocationName.StartsWith("Redmond, "));
            Assert.IsNotNull(queryVM);

            var nameModel = locationProvider.GetLocationFromName(queryVM).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(nameModel);
        }

        [TestMethod]
        public void GetMeteoFranceWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MeteoFrance);
            var weather = GetWeather(provider, new WeatherUtils.Coordinate(48.85, 2.34)).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetWeatherApiWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.WeatherApi);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
        }
    }
}