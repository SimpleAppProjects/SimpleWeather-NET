namespace SimpleWeather.Extras
{
    public interface IExtrasService
    {
        void EnableExtras();
        void DisableExtras();
        bool IsEnabled();
        bool IsIconPackSupported(string packKey);
        bool IsWeatherAPISupported(string api);
        bool IsPremiumWeatherAPI(string api);
        void CheckPremiumStatus();

        bool AreSubscriptionsSupported { get; }
    }
}
