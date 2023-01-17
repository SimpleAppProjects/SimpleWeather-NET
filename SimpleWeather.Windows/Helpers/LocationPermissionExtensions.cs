using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace SimpleWeather.NET.Helpers
{
    public static class LocationPermissionExtensions
    {
        public static async Task<bool> LocationPermissionEnabled(this Page _)
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

        public static IAsyncOperation<bool> LaunchLocationSettings(this Page _)

        {
            return Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }
    }
}
