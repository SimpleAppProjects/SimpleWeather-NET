using SimpleWeather.Utils;
using SimpleWeather.UWP.Controls;
using SimpleWeather.UWP.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Preferences
{
    public sealed class DevSettingsController
    {
        private const int TAPS_TO_BE_A_DEVELOPER = 7;

        private int DevHitCountDown = 0;

        public void OnStart()
        {
            DevHitCountDown = DevSettingsEnabler.DevSettingsEnabled ? -1 : TAPS_TO_BE_A_DEVELOPER;
        }

        public void OnClick()
        {
            if (DevHitCountDown > 0)
            {
                DevHitCountDown--;
                if (DevHitCountDown == 0)
                {
                    Shell.Instance.DismissAllSnackbars();
                    Shell.Instance.ShowSnackbar(Snackbar.MakeSuccess("Dev settings enabled.", SnackbarDuration.Long));
                    EnableDevSettings();
                }
                else if (DevHitCountDown is > 0 and < (TAPS_TO_BE_A_DEVELOPER - 2))
                {
                    Shell.Instance.DismissAllSnackbars();
                    Shell.Instance.ShowSnackbar(Snackbar.Make($"You are now {DevHitCountDown} steps away from being a developer.", SnackbarDuration.VeryShort));
                }
                else if (DevHitCountDown < 0)
                {
                    Shell.Instance.DismissAllSnackbars();
                    Shell.Instance.ShowSnackbar(Snackbar.Make("Dev settings already enabled.", SnackbarDuration.Long));
                }
            }
        }

        private void EnableDevSettings()
        {
            DevHitCountDown = 0;
            DevSettingsEnabler.DevSettingsEnabled = true;
        }
    }
}
