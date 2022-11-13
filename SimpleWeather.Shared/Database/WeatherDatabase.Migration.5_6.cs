using SimpleWeather.WeatherData;
using SQLite;

namespace SimpleWeather.Database
{
    public partial class WeatherDatabase
    {
        private static void Migrate_5_6(SQLiteConnection weatherDB)
        {
            if (weatherDB.Table<Weather>().Count() <= 0)
            {
                return;
            }

            weatherDB.RunInTransaction(() =>
            {
                // Update data
                var weatherToUpdate = weatherDB.Table<Weather>()
                    .Where(w => w.source == WeatherAPI.OpenWeatherMap || w.source == WeatherAPI.MetNo);

                foreach (var weather in weatherToUpdate)
                {
                    var query = weather.query;

                    // Update forecasts
                    weatherDB.Execute("UPDATE forecasts SET forecastblob = REPLACE(forecastblob, ?, ?) WHERE query = ?",
                        "\\\"pop\\\"", "\\\"cloudiness\\\"", query);
                    weatherDB.Execute("UPDATE hr_forecasts SET hrforecastblob = REPLACE(hrforecastblob, ?, ?) WHERE query = ?",
                        "\"pop\"", "\"cloudiness\"", query);

                    // Update weather data
                    weatherDB.Execute("UPDATE weatherdata SET precipitationblob = REPLACE(precipitationblob, ?, ?) WHERE query = ?",
                        "\"pop\"", "\"cloudiness\"", query);
                }
            });
        }
    }
}