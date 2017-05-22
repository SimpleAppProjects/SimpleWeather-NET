using System;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using SimpleWeather.Utils;
using System.Xml.Serialization;
#if WINDOWS_UWP
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
        public static async Task<place> getLocation(WeatherUtils.Coordinate coord)
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
                string content = await response.Content.ReadAsStringAsync();
                byte[] buff = Encoding.UTF8.GetBytes(content);

                // Write array/buffer to memorystream
                MemoryStream memStream = new MemoryStream();
                memStream.Write(buff, 0, buff.Length);
                memStream.Seek(0, 0);

                // End Stream
                webClient.Dispose();

                // Load data
                XmlSerializer deserializer = new XmlSerializer(typeof(query), null, null, new XmlRootAttribute("query"), "");
                query root = (query)deserializer.Deserialize(memStream);

                if (root.results != null)
                    result = root.results[0];
            }
            catch (Exception ex)
            {
                result = null;
#if WINDOWS_UWP
                if (Windows.Web.WebError.GetStatus(ex.HResult) > Windows.Web.WebErrorStatus.Unknown)
                {
                    wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                    await Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                        async () => await new Windows.UI.Popups.MessageDialog(wEx.Message).ShowAsync());
                }
#elif __ANDROID__
                if (ex is System.Net.WebException)
                {
                    System.Net.WebException webEx = ex as System.Net.WebException;
                    if (webEx.Status > System.Net.WebExceptionStatus.Success)
                    {
                        wEx = new WeatherException(WeatherUtils.ErrorStatus.NETWORKERROR);
                        Toast.MakeText(App.Context, wEx.Message, ToastLength.Short).Show();
                    }
                }
#endif
            }

            return result;
        }
    }
}