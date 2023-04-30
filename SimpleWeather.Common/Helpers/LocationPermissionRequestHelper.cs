#if __IOS__
using System;
using System.Threading.Tasks;
using CoreLocation;
using SimpleWeather.Utils;

namespace SimpleWeather.Common.Helpers
{
    public static class LocationPermissionRequestHelper
	{
        private static readonly Lazy<CLLocationManager> clLmgrLazy = new(() =>
        {
            return new CLLocationManager();
        });

        public static CLLocationManager GetLocationManager() => clLmgrLazy.Value;

		public static CLAuthorizationStatus GetAuthorizationStatus(this CLLocationManager locationManager)
		{
            if (OperatingSystem.IsIOSVersionAtLeast(14))
            {
                return locationManager.AuthorizationStatus;
            }
            else
            {
                return CLLocationManager.Status;
            }
        }

        public static bool IsAuthorized(this CLAuthorizationStatus status)
        {
            return status == CLAuthorizationStatus.Authorized ||
                status == CLAuthorizationStatus.AuthorizedWhenInUse ||
                status == CLAuthorizationStatus.AuthorizedAlways;
        }
    }
}
#endif
