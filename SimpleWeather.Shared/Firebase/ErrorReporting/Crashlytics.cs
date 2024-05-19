#if WINDOWS && false
using CommunityToolkit.WinUI.Helpers;
using SimpleWeather.Firebase.ErrorReporting;
using SimpleWeather.Utils;
using System;
using Windows.ApplicationModel;

namespace SimpleWeather.Firebase
{
    public sealed class Crashlytics
    {
        private const string BASE_URL = "https://clouderrorreporting.googleapis.com";

        private readonly string ProjectId;
        private readonly string ApiKey;

        internal Crashlytics(string projectId, string apiKey)
        {
            this.ProjectId = projectId;
            this.ApiKey = apiKey;
        }

        private string GetReportUrl()
        {
            return $"{BASE_URL}/v1beta1/projects/{ProjectId}/events:report";
        }

        public void RecordException(Exception ex)
        {

        }
    }

    internal static class CrashlyticsExtensions
    {
        public static ReportedErrorEvent ToErrorEvent(this Exception ex)
        {
            return new ReportedErrorEvent()
            {
                eventTime = DateTimeOffset.UtcNow.UtcDateTime.ToISO8601Format(),
                message = ex.ToString(),
                context = new ErrorContext()
                {
                    reportLocation = new SourceLocation()
                    {
                        filePath = ex.TargetSite?.DeclaringType?.FullName,
                        functionName = ex.TargetSite?.Name,
                        lineNumber = 0
                    }
                },
                serviceContext = new ServiceContext()
                {
#if DEBUG
                    service = "SimpleWeather.NET.Debug",
#else
                    service = "SimpleWeather.NET",
#endif
                    version = PackageVersionHelper.ToFormattedString(Package.Current.Id.Version),
                }
            };
        }
    }
}
#endif