using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Extras
{
    public interface IExtrasService
    {
        void EnableExtras();
        void DisableExtras();
        bool IsEnabled();
        bool IsIconPackSupported(string packKey);
        bool IsWeatherAPISupported(string api);
        void CheckPremiumStatus();
    }
}
