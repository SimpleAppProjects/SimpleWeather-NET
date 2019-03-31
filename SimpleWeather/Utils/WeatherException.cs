using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleWeather.UWP;

namespace SimpleWeather.Utils
{
    [Serializable]
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
                    errorMsg = App.ResLoader.GetString("WError_NoWeather");
                    break;
                case WeatherUtils.ErrorStatus.NetworkError:
                    errorMsg = App.ResLoader.GetString("WError_NetworkError");
                    break;
                case WeatherUtils.ErrorStatus.InvalidAPIKey:
                    errorMsg = App.ResLoader.GetString("WError_InvalidKey");
                    break;
                case WeatherUtils.ErrorStatus.QueryNotFound:
                    errorMsg = App.ResLoader.GetString("WError_QueryNotFound");
                    break;
                case WeatherUtils.ErrorStatus.Unknown:
                default:
                    errorMsg = App.ResLoader.GetString("WError_Unknown");
                    break;
            }

            return errorMsg;
        }
    }
}
