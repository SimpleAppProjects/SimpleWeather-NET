namespace SimpleWeather.WeatherData.Auth
{
    public abstract class ProviderKey
    {
        public abstract void FromString(string input);
        public abstract override string ToString();
    }
}
