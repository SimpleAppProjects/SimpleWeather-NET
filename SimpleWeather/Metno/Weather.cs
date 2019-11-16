namespace SimpleWeather.Metno
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class weatherdata
    {

        private weatherdataModel[] metaField;

        private weatherdataProduct productField;

        private System.DateTime createdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("model", IsNullable = false)]
        public weatherdataModel[] meta
        {
            get
            {
                return this.metaField;
            }
            set
            {
                this.metaField = value;
            }
        }

        /// <remarks/>
        public weatherdataProduct product
        {
            get
            {
                return this.productField;
            }
            set
            {
                this.productField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime created
        {
            get
            {
                return this.createdField;
            }
            set
            {
                this.createdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataModel
    {

        private string nameField;

        private System.DateTime terminField;

        private System.DateTime runendedField;

        private System.DateTime nextrunField;

        private System.DateTime fromField;

        private System.DateTime toField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime termin
        {
            get
            {
                return this.terminField;
            }
            set
            {
                this.terminField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime runended
        {
            get
            {
                return this.runendedField;
            }
            set
            {
                this.runendedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime nextrun
        {
            get
            {
                return this.nextrunField;
            }
            set
            {
                this.nextrunField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime from
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime to
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProduct
    {

        private weatherdataProductTime[] timeField;

        private string classField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("time")]
        public weatherdataProductTime[] time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTime
    {

        private weatherdataProductTimeLocation locationField;

        private string datatypeField;

        private System.DateTime fromField;

        private System.DateTime toField;

        /// <remarks/>
        public weatherdataProductTimeLocation location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string datatype
        {
            get
            {
                return this.datatypeField;
            }
            set
            {
                this.datatypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime from
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime to
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocation
    {

        private weatherdataProductTimeLocationPrecipitation precipitationField;

        private weatherdataProductTimeLocationMinTemperature minTemperatureField;

        private weatherdataProductTimeLocationMaxTemperature maxTemperatureField;

        private weatherdataProductTimeLocationSymbol symbolField;

        private weatherdataProductTimeLocationSymbolProbability symbolProbabilityField;

        private weatherdataProductTimeLocationTemperature temperatureField;

        private weatherdataProductTimeLocationWindDirection windDirectionField;

        private weatherdataProductTimeLocationWindSpeed windSpeedField;

        private weatherdataProductTimeLocationWindGust windGustField;

        private weatherdataProductTimeLocationAreaMaxWindSpeed areaMaxWindSpeedField;

        private weatherdataProductTimeLocationHumidity humidityField;

        private weatherdataProductTimeLocationPressure pressureField;

        private weatherdataProductTimeLocationCloudiness cloudinessField;

        private weatherdataProductTimeLocationFog fogField;

        private weatherdataProductTimeLocationLowClouds lowCloudsField;

        private weatherdataProductTimeLocationMediumClouds mediumCloudsField;

        private weatherdataProductTimeLocationHighClouds highCloudsField;

        private weatherdataProductTimeLocationTemperatureProbability temperatureProbabilityField;

        private weatherdataProductTimeLocationWindProbability windProbabilityField;

        private weatherdataProductTimeLocationDewpointTemperature dewpointTemperatureField;

        private ushort altitudeField;

        private decimal latitudeField;

        private decimal longitudeField;

        /// <remarks/>
        public weatherdataProductTimeLocationPrecipitation precipitation
        {
            get
            {
                return this.precipitationField;
            }
            set
            {
                this.precipitationField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationMinTemperature minTemperature
        {
            get
            {
                return this.minTemperatureField;
            }
            set
            {
                this.minTemperatureField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationMaxTemperature maxTemperature
        {
            get
            {
                return this.maxTemperatureField;
            }
            set
            {
                this.maxTemperatureField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationSymbol symbol
        {
            get
            {
                return this.symbolField;
            }
            set
            {
                this.symbolField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationSymbolProbability symbolProbability
        {
            get
            {
                return this.symbolProbabilityField;
            }
            set
            {
                this.symbolProbabilityField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationTemperature temperature
        {
            get
            {
                return this.temperatureField;
            }
            set
            {
                this.temperatureField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationWindDirection windDirection
        {
            get
            {
                return this.windDirectionField;
            }
            set
            {
                this.windDirectionField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationWindSpeed windSpeed
        {
            get
            {
                return this.windSpeedField;
            }
            set
            {
                this.windSpeedField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationWindGust windGust
        {
            get
            {
                return this.windGustField;
            }
            set
            {
                this.windGustField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationAreaMaxWindSpeed areaMaxWindSpeed
        {
            get
            {
                return this.areaMaxWindSpeedField;
            }
            set
            {
                this.areaMaxWindSpeedField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationHumidity humidity
        {
            get
            {
                return this.humidityField;
            }
            set
            {
                this.humidityField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationPressure pressure
        {
            get
            {
                return this.pressureField;
            }
            set
            {
                this.pressureField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationCloudiness cloudiness
        {
            get
            {
                return this.cloudinessField;
            }
            set
            {
                this.cloudinessField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationFog fog
        {
            get
            {
                return this.fogField;
            }
            set
            {
                this.fogField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationLowClouds lowClouds
        {
            get
            {
                return this.lowCloudsField;
            }
            set
            {
                this.lowCloudsField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationMediumClouds mediumClouds
        {
            get
            {
                return this.mediumCloudsField;
            }
            set
            {
                this.mediumCloudsField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationHighClouds highClouds
        {
            get
            {
                return this.highCloudsField;
            }
            set
            {
                this.highCloudsField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationTemperatureProbability temperatureProbability
        {
            get
            {
                return this.temperatureProbabilityField;
            }
            set
            {
                this.temperatureProbabilityField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationWindProbability windProbability
        {
            get
            {
                return this.windProbabilityField;
            }
            set
            {
                this.windProbabilityField = value;
            }
        }

        /// <remarks/>
        public weatherdataProductTimeLocationDewpointTemperature dewpointTemperature
        {
            get
            {
                return this.dewpointTemperatureField;
            }
            set
            {
                this.dewpointTemperatureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort altitude
        {
            get
            {
                return this.altitudeField;
            }
            set
            {
                this.altitudeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal latitude
        {
            get
            {
                return this.latitudeField;
            }
            set
            {
                this.latitudeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal longitude
        {
            get
            {
                return this.longitudeField;
            }
            set
            {
                this.longitudeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationPrecipitation
    {

        private string unitField;

        private decimal valueField;

        private decimal minvalueField;

        private bool minvalueFieldSpecified;

        private decimal maxvalueField;

        private bool maxvalueFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal minvalue
        {
            get
            {
                return this.minvalueField;
            }
            set
            {
                this.minvalueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool minvalueSpecified
        {
            get
            {
                return this.minvalueFieldSpecified;
            }
            set
            {
                this.minvalueFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal maxvalue
        {
            get
            {
                return this.maxvalueField;
            }
            set
            {
                this.maxvalueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxvalueSpecified
        {
            get
            {
                return this.maxvalueFieldSpecified;
            }
            set
            {
                this.maxvalueFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationMinTemperature
    {

        private string idField;

        private string unitField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationMaxTemperature
    {

        private string idField;

        private string unitField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationSymbol
    {

        private string idField;

        private byte numberField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationSymbolProbability
    {

        private string unitField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte value
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationTemperature
    {

        private string idField;

        private string unitField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationWindDirection
    {

        private string idField;

        private decimal degField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal deg
        {
            get
            {
                return this.degField;
            }
            set
            {
                this.degField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationWindSpeed
    {

        private string idField;

        private decimal mpsField;

        private byte beaufortField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal mps
        {
            get
            {
                return this.mpsField;
            }
            set
            {
                this.mpsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte beaufort
        {
            get
            {
                return this.beaufortField;
            }
            set
            {
                this.beaufortField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationWindGust
    {

        private string idField;

        private decimal mpsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal mps
        {
            get
            {
                return this.mpsField;
            }
            set
            {
                this.mpsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationAreaMaxWindSpeed
    {

        private decimal mpsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal mps
        {
            get
            {
                return this.mpsField;
            }
            set
            {
                this.mpsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationHumidity
    {

        private decimal valueField;

        private string unitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationPressure
    {

        private string idField;

        private string unitField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationCloudiness
    {

        private string idField;

        private decimal percentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal percent
        {
            get
            {
                return this.percentField;
            }
            set
            {
                this.percentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationFog
    {

        private string idField;

        private decimal percentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal percent
        {
            get
            {
                return this.percentField;
            }
            set
            {
                this.percentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationLowClouds
    {

        private string idField;

        private decimal percentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal percent
        {
            get
            {
                return this.percentField;
            }
            set
            {
                this.percentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationMediumClouds
    {

        private string idField;

        private decimal percentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal percent
        {
            get
            {
                return this.percentField;
            }
            set
            {
                this.percentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationHighClouds
    {

        private string idField;

        private decimal percentField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal percent
        {
            get
            {
                return this.percentField;
            }
            set
            {
                this.percentField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationTemperatureProbability
    {

        private string unitField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte value
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationWindProbability
    {

        private string unitField;

        private byte valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte value
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
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class weatherdataProductTimeLocationDewpointTemperature
    {

        private string idField;

        private string unitField;

        private decimal valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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
}