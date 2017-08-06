using System;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using SimpleWeather.Utils;
using System.Xml.Serialization;
#if WINDOWS_UWP
using SimpleWeather.UWP.Controls;
using Windows.Web.Http;
#elif __ANDROID__
using Android.Widget;
using SimpleWeather.Droid;
using System.Net.Http;
#endif

namespace SimpleWeather.WeatherYahoo
{
    public static class GeopositionQuery
    {
        public static async Task<place> GetLocation(WeatherUtils.Coordinate coord)
        {
            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string location_query = string.Format("({0},{1})", coord.Latitude, coord.Longitude);
            string query = "select * from geo.places where text=\"" + location_query + "\"";
            Uri queryURL = new Uri(yahooAPI + query);
            place result = null;
            WeatherException wEx = null;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                Stream contentStream = null;
#if WINDOWS_UWP
                contentStream = WindowsRuntimeStreamExtensions.AsStreamForRead(await response.Content.ReadAsInputStreamAsync());
#elif __ANDROID__
                contentStream = await response.Content.ReadAsStreamAsync();
#endif

                // End Stream
                webClient.Dispose();

                // Load data
                XmlSerializer deserializer = new XmlSerializer(typeof(query));
                query root = (query)deserializer.Deserialize(contentStream);

                if (root.results != null)
                    result = root.results[0];

                // End Stream
                contentStream.Dispose();
            }
            catch (Exception ex)
            {
                result = null;
#if WINDOWS_UWP
                if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                    Toast.ShowToast(wEx.Message, Toast.ToastDuration.Short);
                }
#elif __ANDROID__
                if (ex is System.Net.WebException || ex is HttpRequestException)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                    new Android.OS.Handler(Android.OS.Looper.MainLooper).Post(() =>
                    {
                        Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
                    });
                }
#endif
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            return result;
        }
    }
}