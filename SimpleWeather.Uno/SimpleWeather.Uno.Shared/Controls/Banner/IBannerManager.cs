namespace SimpleWeather.UWP.Controls
{
    public interface IBannerManager : IBannerPage
    {
        void InitBannerManager();

        void DismissBanner();

        void UnloadBannerManager();
    }
}
