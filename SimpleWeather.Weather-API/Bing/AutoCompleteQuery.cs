namespace SimpleWeather.Weather_API.Bing
{
    public class AC_Rootobject
    {
        public string authenticationResultCode { get; set; }
        public string brandLogoUri { get; set; }
        public string copyright { get; set; }
        public Resourceset[] resourceSets { get; set; }
        public int statusCode { get; set; }
        public string statusDescription { get; set; }
        public string traceId { get; set; }
    }

    public class Resourceset
    {
        public int estimatedTotal { get; set; }
        public Resource[] resources { get; set; }
    }

    public class Resource
    {
        public string __type { get; set; }
        public Value[] value { get; set; }
    }

    public class Value
    {
        public string __type { get; set; }
        public Address address { get; set; }
    }

    public class Address
    {
        public string countryRegion { get; set; }
        public string locality { get; set; }
        public string adminDistrict { get; set; }
        public string countryRegionIso2 { get; set; }
        public string formattedAddress { get; set; }
        public string adminDistrict2 { get; set; }
        public string houseNumber { get; set; }
        public string streetName { get; set; }
        public string addressLine { get; set; }
        public string neighborhood { get; set; }
        public string postalCode { get; set; }
        public string entityType { get; set; }
        public string name { get; set; }
    }
}
