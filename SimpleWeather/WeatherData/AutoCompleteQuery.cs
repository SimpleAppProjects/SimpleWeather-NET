using SimpleWeather.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SimpleWeather.WeatherData
{
    public static class AutoCompleteQuery
    {
        public static async Task<ObservableCollection<Controls.LocationQueryViewModel>> getLocations(string query)
        {
            ObservableCollection<Controls.LocationQueryViewModel> locations = null;

            if (Settings.API == "WUnderground")
            {
                List<Controls.LocationQueryViewModel> ac_query = await WeatherUnderground.AutoCompleteQuery.getLocations(query);

                if (ac_query == null || ac_query.Count == 0)
                    locations = new ObservableCollection<Controls.LocationQueryViewModel>() { new Controls.LocationQueryViewModel() };
                else
                    locations = new ObservableCollection<Controls.LocationQueryViewModel>(ac_query);
            }
            else
            {
                List<Controls.LocationQueryViewModel> ac_query = await WeatherYahoo.AutoCompleteQuery.getLocations(query);

                if (ac_query == null || ac_query.Count == 0)
                    locations = new ObservableCollection<Controls.LocationQueryViewModel>() { new Controls.LocationQueryViewModel() };
                else
                    locations = new ObservableCollection<Controls.LocationQueryViewModel>(ac_query);
            }

            return locations;
        }
    }
}