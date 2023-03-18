using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.NET.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;

namespace SimpleWeather.Maui.Preferences
{
    public sealed class DevSettingsController
    {
        private const int TAPS_TO_BE_A_DEVELOPER = 7;
        private const string KEY_DEVSETTINGS = "key_devsettings";

        public event EventHandler DevSettingsEnabled;
        private int DevHitCountDown = 0;

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private readonly ISnackbarManager snackbarManager;

        internal DevSettingsController(ISnackbarManager snackbarManager)
        {
            this.snackbarManager = snackbarManager;
        }

        public void OnCreatePreferences(TableSection section)
        {
            section.Add(
                new TextCell()
                {
                    Text = "Developer settings",
                    AutomationId = KEY_DEVSETTINGS
                }.Apply(it =>
                {
                    it.Tapped += async (s, e) =>
                    {
                        await App.Current.Navigation.PushAsync(new DevSettingsPage());
                    };
                })
            );
        }

        public void OnStart()
        {
            DevHitCountDown = SettingsManager.DevSettingsEnabled ? -1 : TAPS_TO_BE_A_DEVELOPER;
        }

        public void OnClick()
        {
            if (DevHitCountDown > 0)
            {
                DevHitCountDown--;
                if (DevHitCountDown == 0)
                {
                    snackbarManager?.DismissAllSnackbars();
                    snackbarManager?.ShowSnackbar(Snackbar.MakeSuccess("Dev settings enabled.", SnackbarDuration.Long));
                    EnableDevSettings();
                }
                else if (DevHitCountDown is > 0 and < (TAPS_TO_BE_A_DEVELOPER - 2))
                {
                    snackbarManager?.DismissAllSnackbars();
                    snackbarManager?.ShowSnackbar(Snackbar.Make($"You are now {DevHitCountDown} steps away from being a developer.", SnackbarDuration.VeryShort));
                }
                else if (DevHitCountDown < 0)
                {
                    snackbarManager?.DismissAllSnackbars();
                    snackbarManager?.ShowSnackbar(Snackbar.Make("Dev settings already enabled.", SnackbarDuration.Long));
                }
            }
        }

        private void EnableDevSettings()
        {
            DevHitCountDown = 0;
            SettingsManager.DevSettingsEnabled = true;
            DevSettingsEnabled?.Invoke(this, EventArgs.Empty);
        }
    }
}
