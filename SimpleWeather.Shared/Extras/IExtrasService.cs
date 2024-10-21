namespace SimpleWeather.Extras
{
    public interface IExtrasService
    {
        bool AreSubscriptionsSupported { get; }

        void CheckPremiumStatus();

        void EnablePremiumAccess();
        void EnableProAccess();
        void DisableExtras();
        void DisableProAccess();
        void DisablePremiumAccess();
        bool IsAtLeastProEnabled();
        bool IsPremiumEnabled();
        bool IsProEnabled();

        bool IsIconPackSupported(string packKey);
        bool IsWeatherAPISupported(string api);
        bool IsPremiumWeatherAPI(string api);
    }
}
