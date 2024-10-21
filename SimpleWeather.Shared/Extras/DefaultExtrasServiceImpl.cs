namespace SimpleWeather.Extras
{
    public class DefaultExtrasServiceImpl : IExtrasService
    {
        public bool AreSubscriptionsSupported => true;

        public void DisableExtras()
        {
            // no-op
        }

        public void DisableProAccess()
        {
            // no-op
        }

        public void DisablePremiumAccess()
        {
            // no-op
        }

        public void EnablePremiumAccess()
        {
            // no-op
        }

        public void EnableProAccess()
        {
            // no-op
        }

        public bool IsAtLeastProEnabled()
        {
            return true;
        }

        public bool IsPremiumEnabled()
        {
            return true;
        }

        public bool IsProEnabled()
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
