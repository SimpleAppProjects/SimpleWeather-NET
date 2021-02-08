using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.NWS.Observation
{
    public class PeriodsItem
    {
        public string name { get; set; }
        public DateTimeOffset startTime { get; set; }
        public string tempLabel { get; set; }
        public string temperature { get; set; }
        public string pop { get; set; }
        public string shortForecast { get; set; }
        public string icon { get; set; }
        public string detailedForecast { get; set; }

        public bool IsDaytime => Equals("High", tempLabel);

        public PeriodsItem(String name, DateTimeOffset startTime, String tempLabel, String temperature, String pop,
                       String shortForecast, String icon, String txtForecast)
        {
            this.name = name;
            this.startTime = startTime;
            this.tempLabel = tempLabel;
            this.temperature = temperature;
            this.pop = pop;
            this.shortForecast = shortForecast;
            this.icon = icon;
            this.detailedForecast = txtForecast;
        }
    }
}
