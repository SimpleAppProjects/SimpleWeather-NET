using System.Collections.Generic;

namespace SimpleWeather.Firebase.RemoteConfig
{
    public class FetchResponse
    {
        public Dictionary<string, string> entries { get; set; }
        public string state { get; set; }
        public string templateVersion { get; set; }
    }
}
