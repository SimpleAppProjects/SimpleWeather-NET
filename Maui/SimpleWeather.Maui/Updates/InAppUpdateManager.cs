namespace SimpleWeather.Maui.Updates
{
    public interface InAppUpdateManager
    {
        Task<bool> CheckIfUpdateAvailable();
        int UpdatePriority { get; }
        bool ShouldStartImmediateUpdate();
        Task<bool> ShouldStartImmediateUpdateFlow();
        Task StartImmediateUpdateFlow();
    }
}
