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
            Settings.LoadIfNeeded();
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

        [TestMethod]
        public void GetHereWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.Here);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
        }

        [TestMethod]
        public void GetYahooWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.Yahoo);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
        }

        [TestMethod]
        public void GetMetNoWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.MetNo);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
        }

        [TestMethod]
        public void GetNWSWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.NWS);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
        }

        [TestMethod]
        public void GetOWMWeather()
        {
            var provider = WeatherManager.GetProvider(WeatherAPI.OpenWeatherMap);
            var weather = GetWeather(provider).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(weather?.IsValid() == true);
        }

        [TestMethod]
        public void GetHEREOAuthToken()
        {
            var token = HEREOAuthUtils.GetBearerToken(true).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsTrue(!String.IsNullOrWhiteSpace(token));
        }
    }
}