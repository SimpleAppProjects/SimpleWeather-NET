namespace SimpleWeather.Maui.Updates
{
    public class DefaultInAppUpdateManagerImpl : InAppUpdateManager
    {
        public int UpdatePriority => -1;

        public Task<bool> CheckIfUpdateAvailable()
        {
            return Task.FromResult(false);
        }

        public bool ShouldStartImmediateUpdate()
        {
            return false;
        }

        public Task<bool> ShouldStartImmediateUpdateFlow()
        {
            return Task.FromResult(false);
        }

        public Task StartImmediateUpdateFlow()
        {
            return Task.CompletedTask;
        }
    }
}
