using CommunityToolkit.WinUI.Helpers;
using Windows.ApplicationModel;

namespace SimpleWeather.NET.Helpers
{
    public static class VersionExtensions
    {
        public static string ToVersionCode(this PackageVersion packageVersion)
        {
            return string.Format("{0}{1:00000}{2:00000}", packageVersion.Major, packageVersion.Minor, packageVersion.Build);
        }

        public static string ToVersionCode(this OSVersion osVersion)
        {
            return string.Format("{0}{1:00000}{2:00000}{3:00000}", osVersion.Major, osVersion.Minor, osVersion.Build, osVersion.Revision);
        }
    }
}
