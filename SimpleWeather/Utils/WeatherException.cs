using SimpleWeather.UWP;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SimpleWeather.Utils
{
    [Serializable]
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "<Pending>")]
    [SuppressMessage("Usage", "CA2229:Implement serialization constructors", Justification = "<Pending>")]
    public class WeatherException : Exception
    {
        public WeatherUtils.ErrorStatus ErrorStatus { get; private set; }

        public WeatherException(WeatherUtils.ErrorStatus errorStatus)
        {
            ErrorStatus = errorStatus;
        }

        public override string Message
        {
            get
            {
                string errorMsg;

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
}