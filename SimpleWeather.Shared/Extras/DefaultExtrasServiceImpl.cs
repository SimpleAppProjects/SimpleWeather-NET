using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Extras
{
    public class DefaultExtrasServiceImpl : IExtrasService
    {
        public bool AreSubscriptionsSupported => true;

        public void DisableExtras()
        {
            // no-op
        }

        public void EnableExtras()
        {
            // no-op
        }

        public bool IsEnabled()
        {
            return true;
        }

        public bool IsIconPackSupported(string packKey)
        {
            return true;
        }

        public bool IsWeatherAPISupported(string api)
        {
            return true;
        }

        public void CheckPremiumStatus()
        {
            // no-op
        }
    }
}
