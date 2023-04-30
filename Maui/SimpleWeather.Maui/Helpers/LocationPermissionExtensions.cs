namespace SimpleWeather.Maui.Helpers
{
    public static class LocationPermissionExtensions
    {
        public static Task<bool> LocationPermissionEnabled(this Page _)
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() != PermissionStatus.Granted)
                {
                    var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    return status == PermissionStatus.Granted || status == PermissionStatus.Limited;
                }
                else
                {
                    return true;
                }
            });
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
