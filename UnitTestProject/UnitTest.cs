using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWeather.HERE;
using SimpleWeather.Keys;
using SimpleWeather.Location;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Settings.LoadIfNeeded().GetAwaiter().GetResult();
        }

        /// <summary>
        /// GetWeather
        /// </summary>
        /// <param name="providerImpl"></param>
        /// <returns></returns>
        /// <exception cref="WeatherException">Ignore.</exception>
        private async Task<Weather> GetWeather(WeatherProviderImpl providerImpl)
        {
            var location = await providerImpl.GetLocation(new WeatherUtils.Coordinate(47.6721646, -122.1706614));
            var locData = new LocationData(location);
            return await providerImpl.GetWeather(locData);
        }

        private async Task<bool> SerializerTest(Weather weather)
        {
            var serialStr = await JSONParser.SerializerAsync(weather);
            var deserialWeather = await JSONParser.DeserializerAsync<Weather>(serialStr);
            var fcast = new Forecasts(weather.query, weather.forecast) { txt_forecast = weather.txt_forecast };
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

        [TestMethod]
        public void GetHereWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.Here);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetYahooWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.Yahoo);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetMetNoWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MetNo);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetNWSWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetOWMWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
            Assert.IsTrue(SerializerTest(weather).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        [TestMethod]
        public void GetHEREOAuthToken()
        {
            var token = HEREOAuthUtils.GetBearerToken(true).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(!String.IsNullOrWhiteSpace(token));
        }
    }
}