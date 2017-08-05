using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UWP
using SimpleWeather.UWP;
#elif __ANDROID__
using SimpleWeather.Droid;
#endif

namespace SimpleWeather.Utils
{
    public class WeatherException : Exception
    {
        private WeatherUtils.ErrorStatus errorStatus;

        public WeatherException(WeatherUtils.ErrorStatus errorStatus)
        {
            this.errorStatus = errorStatus;
        }

        public override string Message => GetMessage();

        public String GetMessage()
        {
            String errorMsg;

            switch (errorStatus)
            {
                case WeatherUtils.ErrorStatus.NOWEATHER:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_NoWeather");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_noweather);
#else
                    errorMsg = "Unable to load weather data!!";
#endif
                    break;
                case WeatherUtils.ErrorStatus.NETWORKERROR:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_NetworkError");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_networkerror);
#else
                    errorMsg = "Network Connection Error!!";
#endif
                    break;
                case WeatherUtils.ErrorStatus.INVALIDAPIKEY:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_InvalidKey");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_invalidkey);
#else
                    errorMsg = "Invalid API Key";
#endif
                    break;
                case WeatherUtils.ErrorStatus.QUERYNOTFOUND:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_QueryNotFound");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_querynotfound);
#else
                    errorMsg = "No cities match your search query";
#endif
                    break;
                case WeatherUtils.ErrorStatus.UNKNOWN:
                default:
                    errorMsg = base.Message;
                    break;
            }

            return errorMsg;
        }
    }
}
