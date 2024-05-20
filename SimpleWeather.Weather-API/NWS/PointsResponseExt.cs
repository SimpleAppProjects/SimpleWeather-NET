using SimpleWeather.Utils;
using SimpleWeather.Weather_API.NWS.Hourly;
using System.Collections.Generic;
using System.Linq;

namespace SimpleWeather.Weather_API.NWS
{
    public static class PointsResponseExt
    {
        public static HourlyForecastResponse ToResponse(this HourlyPointsRootobject hourlyRoot, PointsRootobject pointsRootobject)
        {
            return new HourlyForecastResponse()
            {
                creationDate = hourlyRoot.updateTime,
                periodsItems = hourlyRoot.periods?.Select(it =>
                {
                    var dt = it.startTime;

                    return new PeriodsItem()
                    {
                        time = [dt.ToInvariantString("h t").ToLowerInvariant()],
                        unixtime = [dt.ToUnixTimeSeconds().ToInvariantString()],
                        periodName = !string.IsNullOrWhiteSpace(it.name) ? it.name : dt.ToInvariantString("dddd") + (it.isDaytime ? " Night" : ""),
                        windChill = [null],
                        windGust = [null],
                        pop = it.probabilityOfPrecipitation?.value?.Let(x => new List<string> { x.ToInvariantString() }),
                        iconLink = [it.icon],
                        relativeHumidity = [it.relativeHumidity?.value?.ToInvariantString()],
                        temperature = [it.temperature?.ToInvariantString()],
                        weather = [it.shortForecast],
                        windDirection = [it.windDirection?.Let(dir => WeatherUtils.GetWindDirection(dir).ToInvariantString())],
                        windSpeed = [it.windSpeed?.Split(" mph")?.FirstOrDefault()],
                        cloudAmount = [null]
                    };
                })?.ToList()
            };
        }
    }
}
