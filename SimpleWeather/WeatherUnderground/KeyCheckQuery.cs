﻿using System;
using System.Threading.Tasks;
using SimpleWeather.Utils;
#if WINDOWS_UWP
using Windows.Storage.Streams;
using Windows.Web.Http;
#elif __ANDROID__
using System.Net.Http;
#endif

namespace SimpleWeather.WeatherUnderground
{
    public static class KeyCheckQuery
    {
        public static async Task<bool> IsValid(string key)
        {
            string queryAPI = "http://api.wunderground.com/api/";
            string query = "/q/NY/New_York.json";
            Uri queryURL = new Uri(queryAPI + key + query);
            bool isValid = false;
            WeatherException wEx = null;

            try
            {
                if (String.IsNullOrWhiteSpace(key))
                    throw (wEx = new WeatherException(WeatherUtils.ErrorStatus.INVALIDAPIKEY));

                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                // Reset exception
                wEx = null;

                // End Stream
                webClient.Dispose();

                // Load data
                Rootobject root = await JSONParser.DeserializerAsync<Rootobject>(content);

                // Check for errors
                if (root.response.error != null)
                {
                    switch (root.response.error.type)
                    {
                        case "keynotfound":
                            wEx = new WeatherException(WeatherUtils.ErrorStatus.INVALIDAPIKEY);
                            isValid = false;
                            break;
                    }
                }
                else
                    isValid = true;
            }
            catch (Exception)
            {
                isValid = false;
            }

            if (wEx != null)
            {
#if WINDOWS_UWP
                await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
#elif __ANDROID__
                Android.Widget.Toast.MakeText(Droid.App.Context, wEx.Message, Android.Widget.ToastLength.Short).Show();
#endif
            }

            return isValid;
        }
    }
}