using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWeather.Weather_API.Google
{
    public class AlertsResponse
    {
        public List<PublicAlerts> weatherAlerts { get; set; }
        public string regionCode { get; set; }
        public string nextPageToken { get; set; }
    }

    public class PublicAlerts
    {
        public string alertId { get; set; }
        public Alerttitle alertTitle { get; set; }
        public string eventType { get; set; }
        public string areaName { get; set; }
        public string polygon { get; set; }
        public string description { get; set; }
        public string severity { get; set; }
        public string certainty { get; set; }
        public string urgency { get; set; }
        public string[] instruction { get; set; }
        public List<Safetyrecommendation> safetyRecommendations { get; set; }
        public string timezoneOffset { get; set; }
        public string startTime { get; set; }
        public string expirationTime { get; set; }
        public Datasource dataSource { get; set; }
    }

    public class Alerttitle
    {
        public string text { get; set; }
        public string languageCode { get; set; }
    }

    public class Datasource
    {
        public string publisher { get; set; }
        public string name { get; set; }
        public string authorityUri { get; set; }
    }

    public class Safetyrecommendation
    {
        public string directive { get; set; }
        public string subtext { get; set; }
    }
}
