using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using System.Globalization;
using Windows.System.UserProfile;

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
                PostDate = string.Format(SimpleLibrary.ResLoader.GetString("DateTime_DayAgo"),
                    (int)Math.Floor(sincePost.TotalDays));
            else if (sincePost.TotalHours >= 1)
                PostDate = string.Format(SimpleLibrary.ResLoader.GetString("DateTime_HrAgo"),
                    (int)Math.Floor(sincePost.TotalHours));
            else if (sincePost.TotalMinutes >= 1)
                PostDate = string.Format(SimpleLibrary.ResLoader.GetString("DateTime_MinAgo"),
                    (int)Math.Floor(sincePost.TotalMinutes));
            else
                PostDate = string.Format(SimpleLibrary.ResLoader.GetString("DateTime_SecAgo"),
                    (int)Math.Floor(sincePost.TotalSeconds));

            var culture = CultureUtils.UserCulture;

            // Format: Monday, June 15, 2009 1:45 PM
            ExpireDate = string.Format("{0} {1} {2:zzz}",
                SimpleLibrary.ResLoader.GetString("DateTime_ValidUntil"),
                WeatherAlert.ExpiresDate.ToString("f", culture),
                WeatherAlert.ExpiresDate);

            Attribution = WeatherAlert.Attribution;

            if (Attribution != null)
            {
                // TODO: this is temporary; will be removed next release
                if (Attribution.Contains("Information provided by "))
                {
                    Attribution = Attribution.Replace("Information provided by ", "");
                }
                Attribution = String.Format("{0} {1}", SimpleLibrary.ResLoader.GetString("Credit_Prefix/Text"), Attribution);
            }
        }
    }
}