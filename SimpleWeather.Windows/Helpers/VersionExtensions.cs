using Windows.ApplicationModel;

namespace SimpleWeather.NET.Helpers
{
    public static class VersionExtensions
    {
        public static string ToVersionCode(this PackageVersion packageVersion)
        {
            return string.Format("{0}{1:00000}{2:00000}", packageVersion.Major, packageVersion.Minor, packageVersion.Build);
        }
    }
}
