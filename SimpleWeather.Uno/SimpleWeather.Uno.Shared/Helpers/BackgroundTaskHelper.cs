using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace SimpleWeather.Uno.Helpers
{
    public static class BackgroundTaskHelper
    {
        public static async Task<bool> IsBackgroundAccessEnabled()
        {
            // Request access
            BackgroundAccessStatus backgroundAccessStatus = BackgroundAccessStatus.Unspecified;

            try
            {
                backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            }
            catch (UnauthorizedAccessException)
            {
                // An access denied exception may be thrown if two requests are issued at the same time
                // For this specific sample, that could be if the user double clicks "Request access"
            }

            return backgroundAccessStatus is BackgroundAccessStatus.AlwaysAllowed or BackgroundAccessStatus.AllowedSubjectToSystemPolicy;
        }
    }
}
