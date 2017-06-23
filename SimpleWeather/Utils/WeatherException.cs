using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    errorMsg = "Unable to load weather data!!";
                    break;
                case WeatherUtils.ErrorStatus.NETWORKERROR:
                    errorMsg = "Network Connection Error!!";
                    break;
                case WeatherUtils.ErrorStatus.INVALIDAPIKEY:
                    errorMsg = "Invalid API Key";
                    break;
                case WeatherUtils.ErrorStatus.QUERYNOTFOUND:
                    errorMsg = "No cities match your search query";
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
