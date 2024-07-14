using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.System.Profile;

namespace SimpleWeather.NET.Shared.Helpers
{
    public static class DeviceTypeHelper
    {
        public enum DeviceTypes
        {
            Mobile,
            Desktop,
            IoT,
            Xbox,
            Other
        }

        public static DeviceTypes DeviceType
        {
            get
            {
                return AnalyticsInfo.VersionInfo.DeviceFamily switch
                {
                    "Windows.Desktop" => DeviceTypes.Desktop,
                    "Windows.Mobile" => DeviceTypes.Mobile,
                    "Windows.Xbox" => DeviceTypes.Xbox,
                    "Windows.IoT" => DeviceTypes.IoT,
                    _ => DeviceTypes.Other,
                };
            }
        }

        public static bool IsTileSupported()
        {
            if (DeviceType == DeviceTypes.Xbox || DeviceType == DeviceTypes.IoT)
                return false;

            return true;
        }

        public static bool IsSecondaryTileSupported()
        {
            if (!IsTileSupported() || !ApiInformation.IsTypePresent("Windows.UI.StartScreen.SecondaryTile"))
                return false;

            return true;
        }

        public static PackageVersion OSVersion
        {
            get
            {
                ulong version = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
                return new PackageVersion
                {
                    Major = (ushort)((version & 0xFFFF000000000000L) >> 48),
                    Minor = (ushort)((version & 0x0000FFFF00000000L) >> 32),
                    Build = (ushort)((version & 0x00000000FFFF0000L) >> 16),
                    Revision = (ushort)(version & 0x000000000000FFFFL)
                };
            }
        }
    }
}
