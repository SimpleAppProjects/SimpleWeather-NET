using System;
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
                System.IO.Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = System.IO.WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif
                // Reset exception
                wEx = null;

                // End Stream
                webClient.Dispose();

                // Load data
                Rootobject root = await JSONParser.DeserializerAsync<Rootobject>(contentStream);

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

                // End Stream
                contentStream.Dispose();
            }
            catch (Exception)
            {
                isValid = false;
            }

            if (wEx != null)
            {
#if WINDOWS_UWP
                UWP.Controls.Toast.ShowToast(wEx.Message, UWP.Controls.Toast.ToastDuration.Short);
#elif __ANDROID__
                new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() =>
                {
                    Android.Widget.Toast.MakeText(Droid.App.Context, wEx.Message, Android.Widget.ToastLength.Short).Show();
                });
#endif
            }

            return isValid;
        }
    }
}