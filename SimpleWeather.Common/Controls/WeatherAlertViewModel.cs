using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

namespace SimpleWeather.Common.Controls
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

        public WeatherAlertViewModel()
        {

        }

        public WeatherAlertViewModel(WeatherAlert WeatherAlert)
        {
            AlertType = WeatherAlert.Type;
            AlertSeverity = WeatherAlert.Severity;
            Title = WeatherAlert.Title;
            Message = WeatherAlert.Message;

            TimeSpan sincePost = (DateTimeOffset.Now - WeatherAlert.Date);

            if (sincePost.TotalDays >= 1)
                PostDate = string.Format(ResStrings.DateTime_DayAgo,
                    (int)Math.Floor(sincePost.TotalDays));
            else if (sincePost.TotalHours >= 1)
                PostDate = string.Format(ResStrings.DateTime_HrAgo,
                    (int)Math.Floor(sincePost.TotalHours));
            else if (sincePost.TotalMinutes >= 1)
                PostDate = string.Format(ResStrings.DateTime_MinAgo,
                    (int)Math.Floor(sincePost.TotalMinutes));
            else
                PostDate = string.Format(ResStrings.DateTime_SecAgo,
                    (int)Math.Floor(sincePost.TotalSeconds));

            var culture = LocaleUtils.GetLocale();

            // Format: Monday, June 15, 2009 1:45 PM
            ExpireDate = string.Format("{0} {1} {2:zzz}",
                ResStrings.datetime_validuntil,
                WeatherAlert.ExpiresDate.ToString("f", culture),
                WeatherAlert.ExpiresDate);

            Attribution = WeatherAlert.Attribution;

            if (Attribution != null)
            {
                Attribution = String.Format("{0} {1}", ResStrings.credit_prefix, Attribution);
            }
        }
    }
}