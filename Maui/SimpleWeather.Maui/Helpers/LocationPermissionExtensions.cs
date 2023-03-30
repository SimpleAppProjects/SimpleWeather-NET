namespace SimpleWeather.Maui.Helpers
{
    public static class LocationPermissionExtensions
    {
        public static async Task<bool> LocationPermissionEnabled(this Page _)
        {
            if (await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() != PermissionStatus.Granted)
            {
                return await Permissions.RequestAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted;
            }
            else
            {
                return true;
            }
        }

        public static Task<bool> LaunchLocationSettings(this Page _)
        {
            return MainThread.InvokeOnMainThreadAsync(() =>
            {
                AppInfo.ShowSettingsUI();
                return true;
            });
        }
    }
}
