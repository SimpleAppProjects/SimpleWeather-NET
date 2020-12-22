using SimpleWeather.Utils;
using SimpleWeather.UWP.BackgroundTasks;
using SimpleWeather.UWP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.UWP.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_Units : Page, IFrameContentPage
    {
        private bool RequestAppTrigger = false;

        public Settings_Units()
        {
            this.InitializeComponent();
            AddUnits();
            RestoreSettings();
        }

        public void OnNavigatedToPage(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("Settings_Units: OnNavigatedToPage");
            RequestAppTrigger = false;
            RestoreSettings();
        }

        public void OnNavigatedFromPage(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public void OnNavigatingFromPage(NavigatingCancelEventArgs e)
        {
            // Trigger background task if necessary
            if (RequestAppTrigger)
            {
                Task.Run(async () => await WeatherUpdateBackgroundTask.RequestAppTrigger());
                RequestAppTrigger = false;
            }
        }

        private void AddUnits()
        {
            var tempValues = Enum.GetValues(typeof(Units.TemperatureUnits));
            foreach (var enumVal in tempValues)
            {
                var tempUnit = (Units.TemperatureUnits)enumVal;
                switch (tempUnit)
                {
                    default:
                    case Units.TemperatureUnits.Fahrenheit:
                        TemperatureUnits.Items.Add(new RadioButton()
                        {
                            Content = "°F",
                            Tag = tempUnit.GetStringValue(),
                            CommandParameter = tempUnit
                        });
                        break;
                    case Units.TemperatureUnits.Celsuis:
                        TemperatureUnits.Items.Add(new RadioButton()
                        {
                            Content = "°C",
                            Tag = tempUnit.GetStringValue(),
                            CommandParameter = tempUnit
                        });
                        break;
                }
            }
            TemperatureUnits.MaxColumns = Math.Max(tempValues.Length, 1);
            TemperatureUnits.SelectionChanged += (s, e) =>
            {
                if (s is muxc.RadioButtons rbs && rbs.SelectedItem is RadioButton item)
                {
                    Settings.TemperatureUnit = ((Units.TemperatureUnits)item.CommandParameter).GetStringValue();
                    RequestAppTrigger = true;
                }
            };

            var speedValues = Enum.GetValues(typeof(Units.SpeedUnits));
            foreach (var enumVal in speedValues)
            {
                var speedUnit = (Units.SpeedUnits)enumVal;
                switch (speedUnit)
                {
                    default:
                    case Units.SpeedUnits.MilesPerHour:
                        SpeedUnits.Items.Add(new RadioButton()
                        {
                            Content = "mph",
                            Tag = speedUnit.GetStringValue(),
                            CommandParameter = speedUnit
                        });
                        break;
                    case Units.SpeedUnits.KilometersPerHour:
                        SpeedUnits.Items.Add(new RadioButton()
                        {
                            Content = "km/h",
                            Tag = speedUnit.GetStringValue(),
                            CommandParameter = speedUnit
                        });
                        break;
                    case Units.SpeedUnits.MetersPerSecond:
                        SpeedUnits.Items.Add(new RadioButton()
                        {
                            Content = "m/s",
                            Tag = speedUnit.GetStringValue(),
                            CommandParameter = speedUnit
                        });
                        break;
                }
            }
            SpeedUnits.MaxColumns = Math.Max(speedValues.Length, 1);
            SpeedUnits.SelectionChanged += (s, e) =>
            {
                if (s is muxc.RadioButtons rbs && rbs.SelectedItem is RadioButton item)
                {
                    Settings.SpeedUnit = ((Units.SpeedUnits)item.CommandParameter).GetStringValue();
                    RequestAppTrigger = true;
                }
            };

            var distanceValues = Enum.GetValues(typeof(Units.DistanceUnits));
            foreach (var enumVal in distanceValues)
            {
                var distanceUnit = (Units.DistanceUnits)enumVal;
                switch (distanceUnit)
                {
                    default:
                    case Units.DistanceUnits.Miles:
                        DistanceUnits.Items.Add(new RadioButton()
                        {
                            Content = "mi",
                            Tag = distanceUnit.GetStringValue(),
                            CommandParameter = distanceUnit
                        });
                        break;
                    case Units.DistanceUnits.Kilometers:
                        DistanceUnits.Items.Add(new RadioButton()
                        {
                            Content = "km",
                            Tag = distanceUnit.GetStringValue(),
                            CommandParameter = distanceUnit
                        });
                        break;
                }
            }
            DistanceUnits.MaxColumns = Math.Max(distanceValues.Length, 1);
            DistanceUnits.SelectionChanged += (s, e) =>
            {
                if (s is muxc.RadioButtons rbs && rbs.SelectedItem is RadioButton item)
                {
                    Settings.DistanceUnit = ((Units.DistanceUnits)item.CommandParameter).GetStringValue();
                    RequestAppTrigger = true;
                }
            };

            var precipitationValues = Enum.GetValues(typeof(Units.PrecipitationUnits));
            foreach (var enumVal in precipitationValues)
            {
                var precipitationUnit = (Units.PrecipitationUnits)enumVal;
                switch (precipitationUnit)
                {
                    default:
                    case Units.PrecipitationUnits.Inches:
                        PrecipitationUnits.Items.Add(new RadioButton()
                        {
                            Content = "in",
                            Tag = precipitationUnit.GetStringValue(),
                            CommandParameter = precipitationUnit
                        });
                        break;
                    case Units.PrecipitationUnits.Millimeters:
                        PrecipitationUnits.Items.Add(new RadioButton()
                        {
                            Content = "mm",
                            Tag = precipitationUnit.GetStringValue(),
                            CommandParameter = precipitationUnit
                        });
                        break;
                }
            }
            PrecipitationUnits.MaxColumns = Math.Max(precipitationValues.Length, 1);
            PrecipitationUnits.SelectionChanged += (s, e) =>
            {
                if (s is muxc.RadioButtons rbs && rbs.SelectedItem is RadioButton item)
                {
                    Settings.PrecipitationUnit = ((Units.PrecipitationUnits)item.CommandParameter).GetStringValue();
                    RequestAppTrigger = true;
                }
            };

            var pressureValues = Enum.GetValues(typeof(Units.PressureUnits));
            foreach (var enumVal in pressureValues)
            {
                var pressureUnit = (Units.PressureUnits)enumVal;
                switch (pressureUnit)
                {
                    default:
                    case Units.PressureUnits.InHg:
                        PressureUnits.Items.Add(new RadioButton()
                        {
                            Content = "inHg",
                            Tag = pressureUnit.GetStringValue(),
                            CommandParameter = pressureUnit
                        });
                        break;
                    case Units.PressureUnits.Millibar:
                        PressureUnits.Items.Add(new RadioButton()
                        {
                            Content = "mb",
                            Tag = pressureUnit.GetStringValue(),
                            CommandParameter = pressureUnit
                        });
                        break;
                }
            }
            PressureUnits.MaxColumns = Math.Max(pressureValues.Length, 1);
            PressureUnits.SelectionChanged += (s, e) =>
            {
                if (s is muxc.RadioButtons rbs && rbs.SelectedItem is RadioButton item)
                {
                    Settings.PressureUnit = ((Units.PressureUnits)item.CommandParameter).GetStringValue();
                    RequestAppTrigger = true;
                }
            };

            ResetTitle.Text = App.ResLoader.GetString("pref_title_resetunits");
            ResetImperialUnits.Content = App.ResLoader.GetString("default_units_imperial");
            ResetMetricUnits.Content = App.ResLoader.GetString("default_units_metric");
        }

        private void RestoreSettings()
        {
            TemperatureUnits.SelectedItem = TemperatureUnits.Items.FirstOrDefault(btn => Equals((btn as RadioButton).Tag, Settings.TemperatureUnit));
            SpeedUnits.SelectedItem = SpeedUnits.Items.FirstOrDefault(btn => Equals((btn as RadioButton).Tag, Settings.SpeedUnit));
            DistanceUnits.SelectedItem = DistanceUnits.Items.FirstOrDefault(btn => Equals((btn as RadioButton).Tag, Settings.DistanceUnit));
            PrecipitationUnits.SelectedItem = PrecipitationUnits.Items.FirstOrDefault(btn => Equals((btn as RadioButton).Tag, Settings.PrecipitationUnit));
            PressureUnits.SelectedItem = PressureUnits.Items.FirstOrDefault(btn => Equals((btn as RadioButton).Tag, Settings.PressureUnit));
        }

        private void ResetImperialUnits_Click(object sender, RoutedEventArgs e)
        {
            Settings.SetDefaultUnits(Units.FAHRENHEIT);
            RestoreSettings();
        }

        private void ResetMetricUnits_Click(object sender, RoutedEventArgs e)
        {
            Settings.SetDefaultUnits(Units.CELSIUS);
            RestoreSettings();
        }
    }
}
