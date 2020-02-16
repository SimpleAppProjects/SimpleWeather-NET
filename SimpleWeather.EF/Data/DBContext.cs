using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimpleWeather.Location;
using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
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
                    location => JSONParser.Serializer(location, typeof(WeatherData.Location)),
                    location => JSONParser.Deserializer<WeatherData.Location>(location));

            modelBuilder.Entity<Weather>()
                .Property(w => w.condition)
                .HasConversion(
                    condition => JSONParser.Serializer(condition, typeof(Condition)),
                    condition => JSONParser.Deserializer<Condition>(condition));

            modelBuilder.Entity<Weather>()
                .Property(w => w.atmosphere)
                .HasConversion(
                    atmosphere => JSONParser.Serializer(atmosphere, typeof(Atmosphere)),
                    atmosphere => JSONParser.Deserializer<Atmosphere>(atmosphere));

            modelBuilder.Entity<Weather>()
                .Property(w => w.astronomy)
                .HasConversion(
                    astronomy => JSONParser.Serializer(astronomy, typeof(Astronomy)),
                    astronomy => JSONParser.Deserializer<Astronomy>(astronomy));

            modelBuilder.Entity<Weather>()
                .Property(w => w.precipitation)
                .HasConversion(
                    precipitation => JSONParser.Serializer(precipitation, typeof(Precipitation)),
                    precipitation => JSONParser.Deserializer<Precipitation>(precipitation));

            modelBuilder.Entity<Forecasts>()
                .Property(f => f.forecast)
                .HasConversion(
                    // Use default [de]serializer for collections as we did with textblobs in SQLite-Net
                    forecast => JsonConvert.SerializeObject(forecast),
                    forecast => JsonConvert.DeserializeObject<IList<Forecast>>(forecast));

            modelBuilder.Entity<Forecasts>()
                .Property(f => f.txt_forecast)
                .HasConversion(
                    // Use default [de]serializer for collections as we did with textblobs in SQLite-Net
                    forecast => JsonConvert.SerializeObject(forecast),
                    forecast => JsonConvert.DeserializeObject<IList<TextForecast>>(forecast));

            modelBuilder.Entity<HourlyForecasts>()
                .HasKey(f => new { f.query, f.dateblob });

            modelBuilder.Entity<HourlyForecasts>()
                .Property(f => f.hr_forecast)
                .HasConversion(
                    forecast => JSONParser.Serializer(forecast, typeof(HourlyForecast)),
                    forecast => JSONParser.Deserializer<HourlyForecast>(forecast));

            modelBuilder.Entity<WeatherAlerts>()
                .Property(a => a.alerts)
                .HasConversion(
                    // Use default [de]serializer for collections as we did with textblobs in SQLite-Net
                    alerts => JsonConvert.SerializeObject(alerts),
                    alerts => JsonConvert.DeserializeObject<IEnumerable<WeatherAlert>>(alerts));
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