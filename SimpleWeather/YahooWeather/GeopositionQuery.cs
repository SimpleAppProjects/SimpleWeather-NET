using System;
using System.Threading.Tasks;
using System.IO;
using Windows.Web.Http;
using System.Collections.Generic;
using System.Text;
using SimpleWeather.Utils;
using Windows.Devices.Geolocation;
using System.Xml.Serialization;

namespace SimpleWeather.WeatherYahoo
{
    public static class GeopositionQuery
    {
        public static async Task<place> getLocation(Geoposition geoPos)
        {
            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string location_query = string.Format("({0},{1})", geoPos.Coordinate.Point.Position.Latitude, geoPos.Coordinate.Point.Position.Longitude);
            string query = "select * from geo.places where text=\"" + location_query + "\"";
            Uri queryURL = new Uri(yahooAPI + query);
            place result = null;

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
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
    }
}