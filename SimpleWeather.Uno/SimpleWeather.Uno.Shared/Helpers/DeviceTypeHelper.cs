using CommunityToolkit.WinUI.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Metadata;
using Windows.System.Profile;

namespace SimpleWeather.Uno.Shared.Helpers
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
                switch (AnalyticsInfo.VersionInfo.DeviceFamily)
                {
                    case "Windows.Desktop":
                        return DeviceTypes.Desktop;
                    case "Windows.Mobile":
                        return DeviceTypes.Mobile;
                    case "Windows.Xbox":
                        return DeviceTypes.Xbox;
                    case "Windows.IoT":
                        return DeviceTypes.IoT;
                    default:
                        return DeviceTypes.Other;
                }
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

        public static OSVersion OSVersion
        {
            get
            {
                ulong version = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
                return new OSVersion
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
