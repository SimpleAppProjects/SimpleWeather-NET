#if __ANDROID__
using Android.App;

[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]

namespace SimpleWeather.Shared
{
}
#endif