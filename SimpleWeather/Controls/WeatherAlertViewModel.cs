using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#if WINDOWS_UWP
using SimpleWeather.UWP;
#elif __ANDROID__
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.Controls
{
    public class WeatherAlertViewModel
    {
        public WeatherAlertType AlertType { get; set; }
        public WeatherAlertSeverity AlertSeverity { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string PostDate { get; set; }
        public string ExpireDate { get; set; }
        public string Attribution { get; set; }

        public WeatherAlertViewModel(WeatherAlert WeatherAlert)
        {
            AlertType = WeatherAlert.Type;
            AlertSeverity = WeatherAlert.Severity;
            Title = WeatherAlert.Title;
            Message = WeatherAlert.Message;

            TimeSpan sincePost = (DateTimeOffset.Now - WeatherAlert.Date);

            if (sincePost.TotalDays >= 1)
#if WINDOWS_UWP
                PostDate = string.Format(App.ResLoader.GetString("DateTime_DayAgo"),
#elif __ANDROID__
                PostDate = App.Context.GetString(Resource.String.datetime_day_ago,
#endif
                    (int)Math.Floor(sincePost.TotalDays));
            else if (sincePost.TotalHours >= 1)
#if WINDOWS_UWP
                PostDate = string.Format(UWP.App.ResLoader.GetString("DateTime_HrAgo"),
#elif __ANDROID__
                PostDate = App.Context.GetString(Resource.String.datetime_hr_ago,
#endif
                    (int)Math.Floor(sincePost.TotalHours));
            else if (sincePost.TotalMinutes >= 1)
#if WINDOWS_UWP
                PostDate = string.Format(UWP.App.ResLoader.GetString("DateTime_MinAgo"),
#elif __ANDROID__
                PostDate = App.Context.GetString(Resource.String.datetime_min_ago,
#endif
                    (int)Math.Floor(sincePost.TotalMinutes));
            else
#if WINDOWS_UWP
                PostDate = string.Format(UWP.App.ResLoader.GetString("DateTime_SecAgo"),
#elif __ANDROID__
                PostDate = App.Context.GetString(Resource.String.datetime_sec_ago,
#endif
                    (int)Math.Floor(sincePost.TotalSeconds));

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new CultureInfo(userlang);
#else
            var culture = CultureInfo.CurrentCulture;
#endif

            // Format: Monday, June 15, 2009 1:45 PM
            ExpireDate = string.Format("{0} {1} {2:zzz}",
#if WINDOWS_UWP
                UWP.App.ResLoader.GetString("DateTime_ValidUntil"),
#elif __ANDROID__
                App.Context.GetString(Resource.String.datetime_validuntil),
#endif
                WeatherAlert.ExpiresDate.ToString("f", culture),
                WeatherAlert.ExpiresDate);

            Attribution = WeatherAlert.Attribution;
        }
    }
}
