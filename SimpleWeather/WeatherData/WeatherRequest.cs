using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.WeatherData
{
    public sealed class WeatherRequest
    {
        private WeatherRequest()
        {
        }

        internal bool ForceRefresh { get; set; }
        internal bool LoadAlerts { get; set; }
        internal bool LoadForecasts { get; set; }
        internal bool ForceLoadSavedData { get; set; }

        internal IWeatherErrorListener ErrorListener { get; set; }

        public sealed class Builder
        {
            private WeatherRequest request;

            public Builder()
            {
                request = new WeatherRequest();
            }

            public Builder ForceRefresh(bool value)
            {
                request.ForceRefresh = value;
                return this;
            }

            public Builder LoadAlerts()
            {
                request.LoadAlerts = true;
                return this;
            }

            public Builder LoadForecasts()
            {
                request.LoadForecasts = true;
                return this;
            }

            public Builder ForceLoadSavedData()
            {
                request.ForceLoadSavedData = true;
                request.ForceRefresh = false;
                return this;
            }

            public Builder SetErrorListener(IWeatherErrorListener listener)
            {
                request.ErrorListener = listener;
                return this;
            }

            public WeatherRequest Build()
            {
                return request;
            }
        }
    }
}