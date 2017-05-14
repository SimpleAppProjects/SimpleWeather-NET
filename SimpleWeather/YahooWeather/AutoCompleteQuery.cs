using System;
using System.Threading.Tasks;
using System.IO;
using Windows.Web.Http;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SimpleWeather.WeatherYahoo
{
    public static class AutoCompleteQuery
    {
        public static async Task<List<place>> getLocations(string location_query)
        {
            string yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=";
            string query = "select * from geo.places where text=\"" + location_query + "\"";
            Uri queryURL = new Uri(yahooAPI + query);
            List<place> locationResults = null;

            try
            {
                // Connect to webstream
                HttpClient webClient = new HttpClient();
                HttpResponseMessage response = await webClient.GetAsync(queryURL);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                byte[] buff = Encoding.UTF8.GetBytes(content);

                // Write array/buffer to memorystream
                MemoryStream memStream = new MemoryStream();
                memStream.Write(buff, 0, buff.Length);
                memStream.Seek(0, 0);

                // End Stream
                webClient.Dispose();

                // Load data
                locationResults = new List<place>();
                XmlSerializer deserializer = new XmlSerializer(typeof(query), null, null, new XmlRootAttribute("query"), "");
                query root = (query)deserializer.Deserialize(memStream);

                foreach (place result in root.results)
                {
                    // Filter: only store city results
                    if (result.placeTypeName.Value == "Town"
                        || result.placeTypeName.Value == "Suburb")
                        locationResults.Add(result);
                    else
                        continue;
                }
            }
            catch (Exception)
            {
                locationResults = new List<place>();
            }

            return locationResults;
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class query
    {

        private place[] resultsField;

        private byte countField;

        private string createdField;

        private string langField;

        /// <remarks/>
        [XmlArrayItemAttribute("place", Namespace = "http://where.yahooapis.com/v1/schema.rng", IsNullable = false)]
        public place[] results
        {
            get
            {
                return this.resultsField;
            }
            set
            {
                this.resultsField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.yahooapis.com/v1/base.rng")]
        public byte count
        {
            get
            {
                return this.countField;
            }
            set
            {
                this.countField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.yahooapis.com/v1/base.rng")]
        public string created
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

        /// <remarks/>
        [XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.yahooapis.com/v1/base.rng")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    [XmlRootAttribute(Namespace = "http://where.yahooapis.com/v1/schema.rng", IsNullable = false)]
    public partial class place
    {

        private string woeidField;

        private placePlaceTypeName placeTypeNameField;

        private string nameField;

        private placeCountry countryField;

        private placeAdmin1 admin1Field;

        private placeAdmin2 admin2Field;

        private object admin3Field;

        private placeLocality1 locality1Field;

        private placeLocality2 locality2Field;

        private placePostal postalField;

        private placeCentroid centroidField;

        private placeBoundingBox boundingBoxField;

        private byte areaRankField;

        private byte popRankField;

        private placeTimezone timezoneField;

        private string langField;

        private string uriField;

        /// <remarks/>
        public string woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
            }
        }

        /// <remarks/>
        public placePlaceTypeName placeTypeName
        {
            get
            {
                return this.placeTypeNameField;
            }
            set
            {
                this.placeTypeNameField = value;
            }
        }

        /// <remarks/>
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
        public placeCountry country
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
        public placeAdmin1 admin1
        {
            get
            {
                return this.admin1Field;
            }
            set
            {
                this.admin1Field = value;
            }
        }

        /// <remarks/>
        public placeAdmin2 admin2
        {
            get
            {
                return this.admin2Field;
            }
            set
            {
                this.admin2Field = value;
            }
        }

        /// <remarks/>
        public object admin3
        {
            get
            {
                return this.admin3Field;
            }
            set
            {
                this.admin3Field = value;
            }
        }

        /// <remarks/>
        public placeLocality1 locality1
        {
            get
            {
                return this.locality1Field;
            }
            set
            {
                this.locality1Field = value;
            }
        }

        /// <remarks/>
        public placeLocality2 locality2
        {
            get
            {
                return this.locality2Field;
            }
            set
            {
                this.locality2Field = value;
            }
        }

        /// <remarks/>
        public placePostal postal
        {
            get
            {
                return this.postalField;
            }
            set
            {
                this.postalField = value;
            }
        }

        /// <remarks/>
        public placeCentroid centroid
        {
            get
            {
                return this.centroidField;
            }
            set
            {
                this.centroidField = value;
            }
        }

        /// <remarks/>
        public placeBoundingBox boundingBox
        {
            get
            {
                return this.boundingBoxField;
            }
            set
            {
                this.boundingBoxField = value;
            }
        }

        /// <remarks/>
        public byte areaRank
        {
            get
            {
                return this.areaRankField;
            }
            set
            {
                this.areaRankField = value;
            }
        }

        /// <remarks/>
        public byte popRank
        {
            get
            {
                return this.popRankField;
            }
            set
            {
                this.popRankField = value;
            }
        }

        /// <remarks/>
        public placeTimezone timezone
        {
            get
            {
                return this.timezoneField;
            }
            set
            {
                this.timezoneField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string lang
        {
            get
            {
                return this.langField;
            }
            set
            {
                this.langField = value;
            }
        }

        /// <remarks/>
        [XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.yahooapis.com/v1/base.rng")]
        public string uri
        {
            get
            {
                return this.uriField;
            }
            set
            {
                this.uriField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placePlaceTypeName
    {

        private byte codeField;

        private string valueField;

        /// <remarks/>
        [XmlAttributeAttribute()]
        public byte code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeCountry
    {

        private string codeField;

        private string typeField;

        private uint woeidField;

        private string valueField;

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
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

        /// <remarks/>
        [XmlAttributeAttribute()]
        public uint woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeAdmin1
    {

        private string codeField;

        private string typeField;

        private uint woeidField;

        private string valueField;

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
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

        /// <remarks/>
        [XmlAttributeAttribute()]
        public uint woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeAdmin2
    {

        private string codeField;

        private string typeField;

        private uint woeidField;

        private string valueField;

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
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

        /// <remarks/>
        [XmlAttributeAttribute()]
        public uint woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeLocality1
    {

        private string typeField;

        private uint woeidField;

        private bool woeidFieldSpecified;

        private string valueField;

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

        /// <remarks/>
        [XmlAttributeAttribute()]
        public uint woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
            }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool woeidSpecified
        {
            get
            {
                return this.woeidFieldSpecified;
            }
            set
            {
                this.woeidFieldSpecified = value;
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeLocality2
    {

        private string typeField;

        private uint woeidField;

        private bool woeidFieldSpecified;

        private string valueField;

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

        /// <remarks/>
        [XmlAttributeAttribute()]
        public uint woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
            }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool woeidSpecified
        {
            get
            {
                return this.woeidFieldSpecified;
            }
            set
            {
                this.woeidFieldSpecified = value;
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placePostal
    {

        private string typeField;

        private uint woeidField;

        private bool woeidFieldSpecified;

        private string valueField;

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

        /// <remarks/>
        [XmlAttributeAttribute()]
        public uint woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
            }
        }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool woeidSpecified
        {
            get
            {
                return this.woeidFieldSpecified;
            }
            set
            {
                this.woeidFieldSpecified = value;
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeCentroid
    {

        private decimal latitudeField;

        private decimal longitudeField;

        /// <remarks/>
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeBoundingBox
    {

        private placeBoundingBoxSouthWest southWestField;

        private placeBoundingBoxNorthEast northEastField;

        /// <remarks/>
        public placeBoundingBoxSouthWest southWest
        {
            get
            {
                return this.southWestField;
            }
            set
            {
                this.southWestField = value;
            }
        }

        /// <remarks/>
        public placeBoundingBoxNorthEast northEast
        {
            get
            {
                return this.northEastField;
            }
            set
            {
                this.northEastField = value;
            }
        }
    }

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeBoundingBoxSouthWest
    {

        private decimal latitudeField;

        private decimal longitudeField;

        /// <remarks/>
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeBoundingBoxNorthEast
    {

        private decimal latitudeField;

        private decimal longitudeField;

        /// <remarks/>
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
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://where.yahooapis.com/v1/schema.rng")]
    public partial class placeTimezone
    {

        private string typeField;

        private uint woeidField;

        private string valueField;

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

        /// <remarks/>
        [XmlAttributeAttribute()]
        public uint woeid
        {
            get
            {
                return this.woeidField;
            }
            set
            {
                this.woeidField = value;
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
}