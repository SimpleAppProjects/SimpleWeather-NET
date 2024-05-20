using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using Xunit;

namespace MauiUnitTestProject
{
    public class LocationUtilsTests
    {
        [Fact]
        public void IsNYC_US()
        {
            var location = new LocationData { tz_long = "America/New_York" };
            var location2 = new LocationData { latitude = 40.7484445, longitude = -73.9882393 };

            Assert.True(LocationUtils.IsUS(location));
            Assert.True(LocationUtils.IsUS(location2));
        }

        [Fact]
        public void IsPuertoRico()
        {
            var location = new LocationData { tz_long = "America/Puerto_Rico" };
            var location2 = new LocationData { latitude = 40.7484445, longitude = -73.9882393 };

            Assert.True(LocationUtils.IsNWSSupported(location));
            Assert.True(LocationUtils.IsNWSSupported(location2));
        }

        [Fact]
        public void IsUSVI()
        {
            var location = new LocationData { tz_long = "America/St_Thomas" };
            var location2 = new LocationData { latitude = 17.726257, longitude = -64.835823 };

            Assert.True(LocationUtils.IsNWSSupported(location));
            Assert.True(LocationUtils.IsNWSSupported(location2));
        }

        [Fact]
        public void IsGuam_US()
        {
            var location = new LocationData { tz_long = "Pacific/Guam" };
            var location2 = new LocationData { latitude = 13.4623618, longitude = 144.7946835 };

            Assert.True(LocationUtils.IsNWSSupported(location));
            Assert.True(LocationUtils.IsNWSSupported(location2));
        }

        [Fact]
        public void IsAmericaSomoa_US()
        {
            var location = new LocationData { tz_long = "Pacific/Pago_Pago" };
            var location2 = new LocationData { latitude = 17.726257, longitude = -64.835823 };

            Assert.True(LocationUtils.IsNWSSupported(location));
            Assert.True(LocationUtils.IsNWSSupported(location2));
        }

        [Fact]
        public void IsCanada()
        {
            var location = new LocationData { tz_long = "America/Montreal" };
            var location2 = new LocationData { latitude = 45.5593046, longitude = -73.8766794 };

            Assert.True(LocationUtils.IsUSorCanada(location));
            Assert.True(LocationUtils.IsUSorCanada(location2));
        }

        [Fact]
        public void IsFrance()
        {
            var location = new LocationData { tz_long = "Europe/Paris" };
            var location2 = new LocationData { latitude = 48.8589384, longitude = 2.2646349 };

            Assert.True(LocationUtils.IsFrance(location));
            Assert.True(LocationUtils.IsFrance(location2));
        }
    }
}
