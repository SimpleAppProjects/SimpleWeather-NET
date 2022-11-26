using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace SimpleWeather.UWP.Helpers
{
    public static class LocationPermissionExtensions
    {
        public static async Task<bool> LocationPermissionEnabled(this Page page)
        {
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
        }

#if WINDOWS_UWP
        public static IAsyncOperation<bool>
#else 
        public static Task<bool>
#endif
        LaunchLocationSettings(this Page page)

        {
            return Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }
    }
}
