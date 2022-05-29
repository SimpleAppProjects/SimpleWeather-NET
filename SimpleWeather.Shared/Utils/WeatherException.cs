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

        public WeatherException(WeatherUtils.ErrorStatus errorStatus, Exception innerException)
            : base("", innerException)
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
                        errorMsg = SharedModule.Instance.ResLoader.GetString("werror_noweather");
                        break;

                    case WeatherUtils.ErrorStatus.NetworkError:
                        errorMsg = SharedModule.Instance.ResLoader.GetString("werror_networkerror");
                        break;

                    case WeatherUtils.ErrorStatus.InvalidAPIKey:
                        errorMsg = SharedModule.Instance.ResLoader.GetString("werror_invalidkey");
                        break;

                    case WeatherUtils.ErrorStatus.QueryNotFound:
                        errorMsg = SharedModule.Instance.ResLoader.GetString("werror_querynotfound");
                        break;

                    case WeatherUtils.ErrorStatus.Unknown:
                    default:
                        errorMsg = SharedModule.Instance.ResLoader.GetString("werror_unknown");
                        break;
                }

                return errorMsg;
            }
        }
    }
}