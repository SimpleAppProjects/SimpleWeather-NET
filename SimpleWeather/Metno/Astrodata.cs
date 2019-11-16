namespace SimpleWeather.Metno
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class astrodata
    {

        private astrodataMeta metaField;

        private astrodataLocation locationField;

        /// <remarks/>
        public astrodataMeta meta
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
        public astrodataLocation location
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataMeta
    {

        private string licenseurlField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string licenseurl
        {
            get
            {
                return this.licenseurlField;
            }
            set
            {
                this.licenseurlField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocation
    {

        private astrodataLocationTime[] timeField;

        private decimal latitudeField;

        private decimal longitudeField;

        private byte heightField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("time")]
        public astrodataLocationTime[] time
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTime
    {

        private astrodataLocationTimeSunrise sunriseField;

        private astrodataLocationTimeSunset sunsetField;

        private astrodataLocationTimeMoonphase moonphaseField;

        private astrodataLocationTimeMoonshadow moonshadowField;

        private astrodataLocationTimeMoonposition moonpositionField;

        private astrodataLocationTimeLow_moon low_moonField;

        private astrodataLocationTimeSolarnoon solarnoonField;

        private astrodataLocationTimeMoonrise moonriseField;

        private astrodataLocationTimeHigh_moon high_moonField;

        private astrodataLocationTimeMoonset moonsetField;

        private astrodataLocationTimeSolarmidnight solarmidnightField;

        private System.DateTime dateField;

        /// <remarks/>
        public astrodataLocationTimeMoonphase moonphase
        {
            get
            {
                return this.moonphaseField;
            }
            set
            {
                this.moonphaseField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeMoonshadow moonshadow
        {
            get
            {
                return this.moonshadowField;
            }
            set
            {
                this.moonshadowField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeMoonposition moonposition
        {
            get
            {
                return this.moonpositionField;
            }
            set
            {
                this.moonpositionField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeLow_moon low_moon
        {
            get
            {
                return this.low_moonField;
            }
            set
            {
                this.low_moonField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeSolarnoon solarnoon
        {
            get
            {
                return this.solarnoonField;
            }
            set
            {
                this.solarnoonField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeMoonrise moonrise
        {
            get
            {
                return this.moonriseField;
            }
            set
            {
                this.moonriseField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeHigh_moon high_moon
        {
            get
            {
                return this.high_moonField;
            }
            set
            {
                this.high_moonField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeMoonset moonset
        {
            get
            {
                return this.moonsetField;
            }
            set
            {
                this.moonsetField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeSolarmidnight solarmidnight
        {
            get
            {
                return this.solarmidnightField;
            }
            set
            {
                this.solarmidnightField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeSunrise sunrise
        {
            get
            {
                return this.sunriseField;
            }
            set
            {
                this.sunriseField = value;
            }
        }

        /// <remarks/>
        public astrodataLocationTimeSunset sunset
        {
            get
            {
                return this.sunsetField;
            }
            set
            {
                this.sunsetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeMoonphase
    {

        private System.DateTime timeField;

        private decimal valueField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeMoonshadow
    {

        private System.DateTime timeField;

        private decimal elevationField;

        private decimal azimuthField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public decimal elevation
        {
            get
            {
                return this.elevationField;
            }
            set
            {
                this.elevationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal azimuth
        {
            get
            {
                return this.azimuthField;
            }
            set
            {
                this.azimuthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeMoonposition
    {

        private System.DateTime timeField;

        private decimal elevationField;

        private decimal azimuthField;

        private decimal rangeField;

        private decimal phaseField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public decimal elevation
        {
            get
            {
                return this.elevationField;
            }
            set
            {
                this.elevationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal azimuth
        {
            get
            {
                return this.azimuthField;
            }
            set
            {
                this.azimuthField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal range
        {
            get
            {
                return this.rangeField;
            }
            set
            {
                this.rangeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal phase
        {
            get
            {
                return this.phaseField;
            }
            set
            {
                this.phaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeSolarmidnight
    {

        private System.DateTime timeField;

        private decimal elevationField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public decimal elevation
        {
            get
            {
                return this.elevationField;
            }
            set
            {
                this.elevationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeHigh_moon
    {

        private System.DateTime timeField;

        private decimal elevationField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public decimal elevation
        {
            get
            {
                return this.elevationField;
            }
            set
            {
                this.elevationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeSunrise
    {

        private System.DateTime timeField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeMoonset
    {

        private System.DateTime timeField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeSolarnoon
    {

        private System.DateTime timeField;

        private decimal elevationField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public decimal elevation
        {
            get
            {
                return this.elevationField;
            }
            set
            {
                this.elevationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeLow_moon
    {

        private System.DateTime timeField;

        private decimal elevationField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public decimal elevation
        {
            get
            {
                return this.elevationField;
            }
            set
            {
                this.elevationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeSunset
    {

        private System.DateTime timeField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class astrodataLocationTimeMoonrise
    {

        private System.DateTime timeField;

        private string descField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime time
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
        public string desc
        {
            get
            {
                return this.descField;
            }
            set
            {
                this.descField = value;
            }
        }
    }
}
