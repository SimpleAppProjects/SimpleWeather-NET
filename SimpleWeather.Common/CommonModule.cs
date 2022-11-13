using SimpleWeather.Common.Migrations;
using SimpleWeather.Database;
using System;
using System.Threading.Tasks;

namespace SimpleWeather.Common
{
    public class CommonModule
    {
        private static readonly Lazy<CommonModule> lazy = new(() =>
        {
            return new CommonModule();
        });

        public static CommonModule Instance => lazy.Value;

        private CommonModule() { }

        public void Initialize()
        {
            Task.Run(async () =>
            {
                await DataMigrations.PerformVersionMigrations(WeatherDatabase.Instance.Connection, LocationsDatabase.Instance.Connection);
            }).Wait();
        }
    }
}
