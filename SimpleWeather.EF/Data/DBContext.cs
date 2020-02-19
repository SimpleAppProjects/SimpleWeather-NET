using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public class WeatherDBContext : DbContext
    {
        public DbSet<Weather> WeatherData { get; set; }
        public DbSet<Forecasts> Forecasts { get; set; }
        public DbSet<HourlyForecasts> HourlyForecasts { get; set; }
        public DbSet<WeatherAlerts> WeatherAlerts { get; set; }

        public WeatherDBContext()
            : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
#if WINDOWS_UWP
            options.UseSqlite($"Data Source={Settings.wtrDBConnStr}");
#else
            options.UseSqlite("Data Source=C:\\Users\\bryan\\Downloads\\DB\\weatherdata.db");
#endif
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Weather>()
                .Property(w => w.location)
                .HasConversion(
                    value => value.ToJson(),
                    value => SimpleWeather.WeatherData.Location.FromJson(new JsonTextReader(new StringReader(value)) { CloseInput = true }));

            modelBuilder.Entity<Weather>()
                .Property(w => w.condition)
                .HasConversion(
                    value => value.ToJson(),
                    value => Condition.FromJson(new JsonTextReader(new StringReader(value)) { CloseInput = true }));

            modelBuilder.Entity<Weather>()
                .Property(w => w.atmosphere)
                .HasConversion(
                    value => value.ToJson(),
                    value => Atmosphere.FromJson(new JsonTextReader(new StringReader(value)) { CloseInput = true }));

            modelBuilder.Entity<Weather>()
                .Property(w => w.astronomy)
                .HasConversion(
                    value => value.ToJson(),
                    value => Astronomy.FromJson(new JsonTextReader(new StringReader(value)) { CloseInput = true }));

            modelBuilder.Entity<Weather>()
                .Property(w => w.precipitation)
                .HasConversion(
                    value => value.ToJson(),
                    value => Precipitation.FromJson(new JsonTextReader(new StringReader(value)) { CloseInput = true }));

            modelBuilder.Entity<Forecasts>()
                .Property(f => f.forecast)
                .HasConversion(
                    value => JSONParser.CustomEnumerableSerializer(value),
                    value => JSONParser.CustomEnumerableDeserializer<Forecast>(value));

            modelBuilder.Entity<Forecasts>()
                .Property(f => f.txt_forecast)
                .HasConversion(
                    value => JSONParser.CustomEnumerableSerializer(value),
                    value => JSONParser.CustomEnumerableDeserializer<TextForecast>(value));

            modelBuilder.Entity<HourlyForecasts>()
                .HasKey(f => new { f.query, f.dateblob });

            modelBuilder.Entity<HourlyForecasts>()
                .Property(f => f.hr_forecast)
                .HasConversion(
                    value => value.ToJson(),
                    value => HourlyForecast.FromJson(new JsonTextReader(new StringReader(value)) { CloseInput = true }));

            modelBuilder.Entity<WeatherAlerts>()
                .Property(a => a.alerts)
                .HasConversion(
                    value => JSONParser.CustomEnumerableSerializer(value),
                    value => JSONParser.CustomEnumerableDeserializer<WeatherAlert>(value));
        }
    }

    public class LocationDBContext : DbContext
    {
        public DbSet<LocationData> Locations { get; set; }
        public DbSet<Favorites> Favorites { get; set; }

        public LocationDBContext()
            : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
#if WINDOWS_UWP
            options.UseSqlite($"Data Source={Settings.locDBConnStr}");
#else
            options.UseSqlite("Data Source=C:\\Users\\bryan\\Downloads\\DB\\locations.db");
#endif
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationData>()
                .Property(l => l.locationType)
                .HasConversion<int>();
        }
    }
}