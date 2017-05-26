using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;

namespace SimpleWeather.Droid.Helpers
{
    public class LocationListener : Java.Lang.Object, ILocationListener
    {
        public event Action<Location> LocationChanged;
        public event Action<string> ProviderDisabled;
        public event Action<string> ProviderEnabled;
        public event Action<string, Availability, Bundle> StatusChanged;

        public void OnLocationChanged(Location location)
        {
            LocationChanged?.Invoke(location);
        }

        public void OnProviderDisabled(string provider)
        {
            ProviderDisabled?.Invoke(provider);
        }

        public void OnProviderEnabled(string provider)
        {
            ProviderEnabled?.Invoke(provider);
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            StatusChanged?.Invoke(provider, status, extras);
        }
    }
}