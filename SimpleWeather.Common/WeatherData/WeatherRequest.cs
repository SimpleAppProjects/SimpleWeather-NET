using System;
using System.Threading;

namespace SimpleWeather.Common.WeatherData
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
        internal bool ShouldSaveData { get; set; } = true;
        internal CancellationToken CancelToken { get; set; }

        /// <summary>
        /// ThrowIfCancellationRequested
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        internal void ThrowIfCancellationRequested()
        {
            if (CancelToken != null)
            {
                CancelToken.ThrowIfCancellationRequested();
            }
        }

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

            public Builder ForceRefreshWithoutSave()
            {
                request.ForceRefresh = true;
                request.ForceLoadSavedData = false;
                request.ShouldSaveData = false;
                return this;
            }

            public Builder SetCancellationToken(CancellationToken token)
            {
                request.CancelToken = token;
                return this;
            }

            public WeatherRequest Build()
            {
                return request;
            }
        }
    }
}