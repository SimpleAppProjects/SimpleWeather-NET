namespace SimpleWeather.Uno.Controls
{
    public interface IBannerManager : IBannerPage
    {
        void InitBannerManager();

        void DismissBanner();

        void UnloadBannerManager();
    }
}
