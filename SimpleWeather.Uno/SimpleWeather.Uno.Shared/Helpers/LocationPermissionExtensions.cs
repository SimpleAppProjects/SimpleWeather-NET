using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.System;
using Microsoft.UI.Xaml.Controls;
#if !(WINDOWS || __MACOS__ || HAS_UNO_SKIA)
using Microsoft.Maui.ApplicationModel;
#endif

namespace SimpleWeather.Uno.Helpers
{
    public static class LocationPermissionExtensions
    {
        public static async Task<bool> LocationPermissionEnabled(this Page _)
        {
#if WINDOWS || __MACOS__ || HAS_UNO_SKIA
            var geoStatus = GeolocationAccessStatus.Unspecified;

            try
            {
                geoStatus = await Geolocator.RequestAccessAsync();
            }
            catch (Exception)
            {
                // Access denied
            }

            return geoStatus == GeolocationAccessStatus.Allowed;
#else
            if (await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() != PermissionStatus.Granted)
            {
                return await Permissions.RequestAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted;
            }
            else
            {
                return true;
            }
#endif
        }

#if WINDOWS
        public static IAsyncOperation<bool>
#else
        public static Task<bool>
#endif
        LaunchLocationSettings(this Page _)

        {
            return Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }
    }
}
