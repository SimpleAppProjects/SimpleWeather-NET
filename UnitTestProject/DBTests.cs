using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace UnitTestProject
{
    [TestClass]
    public class DBTests
    {
        // App data files
        private static StorageFolder appDataFolder = ApplicationData.Current.LocalFolder;
        private SQLiteAsyncConnection weatherDB;

        [TestInitialize]
        public void InitDB()
        {
            if (weatherDB == null)
            {
                weatherDB = new SQLiteAsyncConnection(
                    Path.Combine(appDataFolder.Path, "test-weatherdata.db"),
                    SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex
                    );
                var conn = weatherDB.GetConnection();
                var _lock = conn.Lock();
                conn.BusyTimeout = TimeSpan.FromSeconds(5);
                conn.EnableWriteAheadLogging();
                _lock.Dispose();
            }

            App.Current.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.Exception);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine(e.Exception);
            };
        }

        [TestCleanup]
        public async Task DestroyDB()
        {
            if (weatherDB != null)
            {
                await weatherDB.CloseAsync();
            }
        }

        [TestMethod]
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

        [IgnoreDataMember]
        public string forecastblob { get; set; }

        [IgnoreDataMember]
        public string txtforecastblob { get; set; }

        public Forecasts_Pre_v510()
        {
        }
    }
}
