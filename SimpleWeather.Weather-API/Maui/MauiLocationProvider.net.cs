#if !__IOS__
using SimpleWeather.LocationData;
using SimpleWeather.Utils;
using SimpleWeather.Weather_API.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.Weather_API.Maui
{
    public partial class MauiLocationProvider : WeatherLocationProviderImpl, IRateLimitedRequest
    {
        public override string LocationAPI => throw new System.NotImplementedException();

        public override Task<ObservableCollection<LocationQuery>> GetLocations(string ac_query, string weatherAPI)
        {
            throw new System.NotImplementedException();
        }

        public override Task<LocationQuery> GetLocation(WeatherUtils.Coordinate coordinate, string weatherAPI)
        {
            throw new System.NotImplementedException();
        }

        public override Task<LocationQuery> GetLocationFromID(LocationQuery model)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif