﻿using SimpleWeather.Helpers;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xunit;

namespace UnitTestProject
{
    public partial class DBTests : IDisposable
    {
        // App data files
        private static string LocalFolderPath = ApplicationDataHelper.GetLocalFolderPath();
        private SQLiteAsyncConnection weatherDB;

        public DBTests()
        {
            if (weatherDB == null)
            {
                weatherDB = new SQLiteAsyncConnection(
                    Path.Combine(LocalFolderPath, "test-weatherdata.db"),
                    SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex
                    );
                var conn = weatherDB.GetConnection();
                var _lock = conn.Lock();
                conn.BusyTimeout = TimeSpan.FromSeconds(5);
                conn.EnableWriteAheadLogging();
                _lock.Dispose();
            }

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.Exception);
            };
        }

        public async void Dispose()
        {
            if (weatherDB != null)
            {
                await weatherDB.CloseAsync();
            }
        }

        [Fact]
        public async Task DBMigrate_v510()
        {
            await weatherDB.DropTableAsync<Forecasts>();
            await weatherDB.CreateTableAsync<Forecasts_Pre_v510>();

            var t1 = Task.Factory.StartNew(() =>
            {
                Task.Delay(5000);
                CreateDatabase();
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            var t2 = Task.Factory.StartNew(() =>
            {
                Task.Delay(5000);
                CreateDatabase();
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            var t3 = Task.Factory.StartNew(() =>
            {
                Task.Delay(5000);
                CreateDatabase();
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            var t4 = Task.Factory.StartNew(() =>
            {
                Task.Delay(5000);
                CreateDatabase();
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            await Task.WhenAll(t1, t2, t3, t4);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CreateDatabase()
        {
            var weatherDBConn = weatherDB.GetConnection();

            using (weatherDBConn.Lock())
            {
                weatherDBConn.CreateTable<Weather>();
                weatherDBConn.CreateTable<WeatherAlerts>();
                weatherDBConn.CreateTable<HourlyForecasts>();
                weatherDBConn.CreateTable<Forecasts>();
            }
        }
    }

    [Table(TABLE_NAME)]
    public class Forecasts_Pre_v510
    {
        public const string TABLE_NAME = "forecasts";

        [PrimaryKey]
        public string query { get; set; }

        [TextBlob(nameof(forecastblob))]
        public IList<Forecast> forecast { get; set; }

        [TextBlob(nameof(txtforecastblob))]
        public IList<TextForecast> txt_forecast { get; set; }

        [JsonIgnore]
        public string forecastblob { get; set; }

        [JsonIgnore]
        public string txtforecastblob { get; set; }

        public Forecasts_Pre_v510()
        {
        }
    }
}
