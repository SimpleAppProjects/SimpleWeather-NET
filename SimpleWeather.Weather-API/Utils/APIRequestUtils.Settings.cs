﻿using SimpleWeather.Preferences;
using System;

namespace SimpleWeather.Weather_API.Utils
{
    public static partial class APIRequestUtils
    {
        // Shared Settings
        private static readonly SettingsContainer localSettings = new SettingsContainer();

        private static partial long GetNextRetryTime(string apiID)
        {
            return localSettings.GetValue<long>(GetRetryTimePrefKey(apiID), -1L);
        }

        private static partial void SetNextRetryTime(string apiID, long retryTimeInMs)
        {
            localSettings.SetValue(GetRetryTimePrefKey(apiID),
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + (retryTimeInMs + GetRandomOffset(retryTimeInMs)));
        }

        private static partial int GetRetryCount(string apiID)
        {
            return localSettings.GetValue<int>(GetRetryCountPrefKey(apiID), 0);
        }

        private static partial void SetRetryCount(string apiID, int count)
        {
            localSettings.SetValue(GetRetryCountPrefKey(apiID), count);
        }
    }
}
