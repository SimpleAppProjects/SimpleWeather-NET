namespace SimpleWeather.Firebase.Analytics
{
    public class ValidationRootobject
    {
        public Validationmessage[] validationMessages { get; set; }
    }

    public class Validationmessage
    {
        public string fieldPath { get; set; }
        public string description { get; set; }
        public string validationCode { get; set; }
    }

}
