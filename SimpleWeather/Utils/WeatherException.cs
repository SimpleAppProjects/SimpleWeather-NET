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
        public WeatherUtils.ErrorStatus ErrorStatus;

        public WeatherException(WeatherUtils.ErrorStatus errorStatus)
        {
            ErrorStatus = errorStatus;
        }

        public override string Message => GetMessage();

        public String GetMessage()
        {
            String errorMsg;

            switch (ErrorStatus)
            {
                case WeatherUtils.ErrorStatus.NoWeather:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_NoWeather");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_noweather);
#else
                    errorMsg = "Unable to load weather data!!";
#endif
                    break;
                case WeatherUtils.ErrorStatus.NetworkError:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_NetworkError");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_networkerror);
#else
                    errorMsg = "Network Connection Error!!";
#endif
                    break;
                case WeatherUtils.ErrorStatus.InvalidAPIKey:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_InvalidKey");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_invalidkey);
#else
                    errorMsg = "Invalid API Key";
#endif
                    break;
                case WeatherUtils.ErrorStatus.QueryNotFound:
#if WINDOWS_UWP
                    errorMsg = App.ResLoader.GetString("WError_QueryNotFound");
#elif __ANDROID__
                    errorMsg = App.Context.GetString(Resource.String.werror_querynotfound);
#else
                    errorMsg = "No cities match your search query";
#endif
                    break;
                case WeatherUtils.ErrorStatus.Unknown:
                default:
                    errorMsg = base.Message;
                    break;
            }

            return errorMsg;
        }
    }
}
