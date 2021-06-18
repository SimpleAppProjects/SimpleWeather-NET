using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleWeather.Utils
{
    public static partial class APIRequestUtils
    {
        // Shared Settings
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private static partial long GetNextRetryTime(string apiID)
        {
            if (localSettings.Values.TryGetValue(GetRetryTimePrefKey(apiID), out object value))
            {
                if (value is long @int)
                {
                    return @int;
                }
            }

            return -1;
        }

        private static partial void SetNextRetryTime(string apiID, long retryTimeInMs)
        {
            localSettings.Values[GetRetryTimePrefKey(apiID)] = 
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (retryTimeInMs + GetRandomOffset(retryTimeInMs));
        }
    }
}
