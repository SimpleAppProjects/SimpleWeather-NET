using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public static partial class FeatureSettings
    {
        private const string KEY_UPDATEAVAILABLE = "key_updateavailable";
        public static bool IsUpdateAvailable { get { return GetUpdateAvailable(); } set { SetUpdateAvailable(value); } }
    }
}