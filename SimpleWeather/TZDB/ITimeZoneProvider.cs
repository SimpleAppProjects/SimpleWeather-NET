using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.TZDB
{
    public interface ITimeZoneProvider
    {
        Task<String> GetTimeZone(double latitude, double longitude);
    }
}
