using SimpleWeather.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public static class AutoCompleteQuery
    {
        public static async Task<ObservableCollection<Controls.LocationQueryView>> getLocations(string query)
        {
            ObservableCollection<Controls.LocationQueryView> locations = null;

            if (Settings.API == "WUnderground")
            {
                List<Controls.LocationQueryView> ac_query = await WeatherUnderground.AutoCompleteQuery.getLocations(query);

                if (ac_query == null || ac_query.Count == 0)
                    locations = new ObservableCollection<Controls.LocationQueryView>() { new Controls.LocationQueryView() };
                else
                    locations = new ObservableCollection<Controls.LocationQueryView>(ac_query);
            }
            else
            {
                List<Controls.LocationQueryView> ac_query = await WeatherYahoo.AutoCompleteQuery.getLocations(query);

                if (ac_query == null || ac_query.Count == 0)
                    locations = new ObservableCollection<Controls.LocationQueryView>() { new Controls.LocationQueryView() };
                else
                    locations = new ObservableCollection<Controls.LocationQueryView>(ac_query);
            }

            return locations;
        }
    }
}