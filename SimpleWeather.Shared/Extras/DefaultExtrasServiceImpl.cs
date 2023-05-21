namespace SimpleWeather.Extras
{
    public class DefaultExtrasServiceImpl : IExtrasService
    {
        public bool AreSubscriptionsSupported => true;

        public void DisableExtras()
        {
            // no-op
        }

        public void EnableExtras()
        {
            // no-op
        }

        public bool IsEnabled()
        {
            return true;
        }

        public bool IsIconPackSupported(string packKey)
        {
            return true;
        }

        public bool IsWeatherAPISupported(string api)
        {
            return true;
        }

        public bool IsPremiumWeatherAPI(string api)
        {
            return false;
        }

        public void CheckPremiumStatus()
        {
            // no-op
        }
    }
}
