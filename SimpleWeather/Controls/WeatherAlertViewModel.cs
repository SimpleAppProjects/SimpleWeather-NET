using SimpleWeather.WeatherData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SimpleWeather.Controls
{
    public class WeatherAlertViewModel
    {
        public WeatherAlertType AlertType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string PostDate { get; set; }
        public string ExpireDate { get; set; }
        public string Attribution { get; set; }

        public WeatherAlertViewModel(WeatherAlert WeatherAlert)
        {
            AlertType = WeatherAlert.Type;
            Title = WeatherAlert.Title;
            Message = WeatherAlert.Message;

            TimeSpan sincePost = (DateTimeOffset.Now - WeatherAlert.Date);

            // TODO: move to resources
            if (sincePost.TotalDays >= 1)
                PostDate = string.Format("{0}d ago", (int)Math.Floor(sincePost.TotalDays));
            else if (sincePost.TotalHours >= 1)
                PostDate = string.Format("{0}h ago", (int)Math.Floor(sincePost.TotalHours));
            else if (sincePost.TotalMinutes >= 1)
                PostDate = string.Format("{0}m ago", (int)Math.Floor(sincePost.TotalMinutes));
            else
                PostDate = string.Format("{0}s ago", (int)Math.Floor(sincePost.TotalSeconds));

#if WINDOWS_UWP
            var userlang = Windows.System.UserProfile.GlobalizationPreferences.Languages[0];
            var culture = new System.Globalization.CultureInfo(userlang);
#else
            var culture = System.Globalization.CultureInfo.CurrentCulture;
#endif
            // TODO: move to resources
            // Format: Monday, June 15, 2009 1:45 PM
            ExpireDate = string.Format("Valid until {0} {1:zzz}",
                WeatherAlert.ExpiresDate.ToString("f", culture),
                WeatherAlert.ExpiresDate);

            Attribution = WeatherAlert.Attribution;
        }
    }
}
