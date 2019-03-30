using System;
using System.Xml.Serialization;

namespace SimpleWeather.WeatherYahoo
{
    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class location
    {

        private locationTermsofservice termsofserviceField;

        private string countryField;

        private string stateField;

        private string cityField;

        private string tz_shortField;

        private string tz_unixField;

        private string latField;

        private string lonField;

        private string zipField;

        private string magicField;

        private string wmoField;

        private string requesturlField;

        private string wuiurlField;

        private locationRadar radarField;

        private locationNearby_weather_stations nearby_weather_stationsField;

        private string typeField;

        public string query
        {
            get { return string.Format("/q/zmw:{0}.{1}.{2}", zip, magic, wmo); }
        }

        /// <remarks/>
        public locationTermsofservice termsofservice
        {
            get
            {
                return this.termsofserviceField;
            }
            set
            {
                this.termsofserviceField = value;
            }
        }

        /// <remarks/>
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string tz_short
        {
            get
            {
                return this.tz_shortField;
            }
            set
            {
                this.tz_shortField = value;
            }
        }

        /// <remarks/>
        public string tz_unix
        {
            get
            {
                return this.tz_unixField;
            }
            set
            {
                this.tz_unixField = value;
            }
        }

        /// <remarks/>
        public string lat
        {
            get
            {
                return this.latField;
            }
            set
            {
                this.latField = value;
            }
        }

        /// <remarks/>
        public string lon
        {
            get
            {
                return this.lonField;
            }
            set
            {
                this.lonField = value;
            }
        }

        /// <remarks/>
        public string zip
        {
            get
            {
                return this.zipField;
            }
            set
            {
                this.zipField = value;
            }
        }

        /// <remarks/>
        public string magic
        {
            get
            {
                return this.magicField;
            }
            set
            {
                this.magicField = value;
            }
        }

        /// <remarks/>
        public string wmo
        {
            get
            {
                return this.wmoField;
            }
            set
            {
                this.wmoField = value;
            }
        }

        /// <remarks/>
        public string requesturl
        {
            get
            {
                return this.requesturlField;
            }
            set
            {
                this.requesturlField = value;
            }
        }

        /// <remarks/>
        public string wuiurl
        {
            get
            {
                return this.wuiurlField;
            }
            set
            {
                this.wuiurlField = value;
            }
        }

        /// <remarks/>
        public locationRadar radar
        {
            get
            {
                return this.radarField;
            }
            set
            {
                this.radarField = value;
            }
        }

        /// <remarks/>
        public locationNearby_weather_stations nearby_weather_stations
        {
            get
            {
                return this.nearby_weather_stationsField;
            }
            set
            {
                this.nearby_weather_stationsField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class locationTermsofservice
    {

        private string linkField;

        private string valueField;

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string link
        {
            get
            {
                return this.linkField;
            }
            set
            {
                this.linkField = value;
            }
        }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class locationRadar
    {

        private string image_urlField;

        private string urlField;

        /// <remarks/>
        public string image_url
        {
            get
            {
                return this.image_urlField;
            }
            set
            {
                this.image_urlField = value;
            }
        }

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class locationNearby_weather_stations
    {

        private locationNearby_weather_stationsStation[] airportField;

        private locationNearby_weather_stationsStation1[] pwsField;

        /// <remarks/>
        [XmlArrayItemAttribute("station", IsNullable = false)]
        public locationNearby_weather_stationsStation[] airport
        {
            get
            {
                return this.airportField;
            }
            set
            {
                this.airportField = value;
            }
        }

        /// <remarks/>
        [XmlArrayItemAttribute("station", IsNullable = false)]
        public locationNearby_weather_stationsStation1[] pws
        {
            get
            {
                return this.pwsField;
            }
            set
            {
                this.pwsField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class locationNearby_weather_stationsStation
    {

        private string cityField;

        private string stateField;

        private string countryField;

        private string icaoField;

        private decimal latField;

        private decimal lonField;

        /// <remarks/>
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string icao
        {
            get
            {
                return this.icaoField;
            }
            set
            {
                this.icaoField = value;
            }
        }

        /// <remarks/>
        public decimal lat
        {
            get
            {
                return this.latField;
            }
            set
            {
                this.latField = value;
            }
        }

        /// <remarks/>
        public decimal lon
        {
            get
            {
                return this.lonField;
            }
            set
            {
                this.lonField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class locationNearby_weather_stationsStation1
    {

        private string neighborhoodField;

        private string cityField;

        private string stateField;

        private string countryField;

        private string idField;

        private string distance_kmField;

        private string distance_miField;

        /// <remarks/>
        public string neighborhood
        {
            get
            {
                return this.neighborhoodField;
            }
            set
            {
                this.neighborhoodField = value;
            }
        }

        /// <remarks/>
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string distance_km
        {
            get
            {
                return this.distance_kmField;
            }
            set
            {
                this.distance_kmField = value;
            }
        }

        /// <remarks/>
        public string distance_mi
        {
            get
            {
                return this.distance_miField;
            }
            set
            {
                this.distance_miField = value;
            }
        }
    }
}
