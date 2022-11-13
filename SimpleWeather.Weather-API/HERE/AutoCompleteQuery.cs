namespace SimpleWeather.Weather_API.HERE
{
    public class AC_Rootobject
    {
        public Suggestion[] suggestions { get; set; }
    }

    public class Suggestion
    {
        public string label { get; set; }
        public string language { get; set; }
        public string countryCode { get; set; }
        public string locationId { get; set; }
        public Address address { get; set; }
        public string matchLevel { get; set; }
    }

    public partial class Address
    {
        public string country { get; set; }
        public string state { get; set; }
        public string county { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string district { get; set; }
        public string street { get; set; }
    }
}