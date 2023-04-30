#if __IOS__
using System;
using CoreLocation;
using Foundation;
using SimpleWeather.Common.Helpers;

namespace SimpleWeather.Maui
{
    public class LocationManagerDelegate : NSObject, ICLLocationManagerDelegate
    {
        public event EventHandler<CLAuthorizationChangedEventArgs> AuthorizationStatusChanged;

        [Export("locationManager:didChangeAuthorizationStatus:")]
        public void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status) =>
            AuthorizationStatusChanged?.Invoke(this, new CLAuthorizationChangedEventArgs(status));

        [Export("locationManagerDidChangeAuthorization:")]
        public void DidChangeAuthorization(CLLocationManager manager) =>
            AuthorizationStatusChanged?.Invoke(this, new CLAuthorizationChangedEventArgs(manager.GetAuthorizationStatus()));
    }
}
#endif