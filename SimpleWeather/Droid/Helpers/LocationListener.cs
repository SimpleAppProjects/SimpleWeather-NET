#if __ANDROID__
using System;
using Android.OS;
using Android.Runtime;
using Android.Locations;

namespace SimpleWeather.Droid.Helpers
{
    public class LocationListener : Java.Lang.Object, ILocationListener
    {
        public event Action<Location> LocationChanged;
        public event Action<string> ProviderDisabled;
        public event Action<string> ProviderEnabled;
        public event Action<string, Availability, Bundle> StatusChanged;

        // Called when the location has changed.
        public void OnLocationChanged(Location location)
        {
            LocationChanged?.Invoke(location);
        }

        // Called when the provider is disabled by the user.
        public void OnProviderDisabled(string provider)
        {
            ProviderDisabled?.Invoke(provider);
        }

        // Called when the provider is enabled by the user.
        public void OnProviderEnabled(string provider)
        {
            ProviderEnabled?.Invoke(provider);
        }

        // Called when the provider status changes.
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            StatusChanged?.Invoke(provider, status, extras);
        }
    }
}
#endif