using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Metadata;
using Windows.System.Profile;

namespace SimpleWeather.UWP.Shared.Helpers
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
            if (DeviceType == DeviceTypes.Xbox || DeviceType == DeviceTypes.IoT ||
                !ApiInformation.IsTypePresent("Windows.ApplicationModel.Background.ToastNotificationActionTrigger"))
                return false;

            return true;
        }
    }
}
