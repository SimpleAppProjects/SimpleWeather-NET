#if WINDOWS
using Windows.ApplicationModel;
#endif

namespace SimpleWeather.Utils
{
    public static class VersionUtils
    {
#if WINDOWS
        public static Version ToVersion(this PackageVersion version)
        {
            return new Version(version.Major, version.Minor, version.Build);
        }
#endif

        public static Version ToVersion(this System.Version version)
        {
            return new Version(version.Major, version.Minor, version.Build < 0 ? 0 : version.Build);
        }
    }
}