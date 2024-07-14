using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SimpleWeather.NET.Helpers;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using ResUnits = SimpleWeather.Resources.Strings.Units;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SimpleWeather.NET.Preferences
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings_Units : Page, IFrameContentPage
    {
        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private bool UnitsChanged = false;

        public Settings_Units()
        {
            this.InitializeComponent();
            AddUnits();
            RestoreSettings();
            RegisterListeners();
        }

        public void OnNavigatedToPage(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AnalyticsLogger.LogEvent("Settings_Units: OnNavigatedToPage");
            UnitsChanged = false;
            RestoreSettings();
        }

        public void OnNavigatedFromPage(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public void OnNavigatingFromPage(NavigatingCancelEventArgs e)
        {
            // Trigger background task if necessary
            if (UnitsChanged)
            {
                SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEUNIT);
                UnitsChanged = false;
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
                        TemperatureUnits.Items.Add(new SegmentedItem()
                        {
                            Content = "°F",
                            Tag = tempUnit.GetStringValue()
                        });
                        break;
                    case Units.TemperatureUnits.Celsuis:
                        TemperatureUnits.Items.Add(new SegmentedItem()
                        {
                            Content = "°C",
                            Tag = tempUnit.GetStringValue()
                        });
                        break;
                }
            }

            var speedValues = Enum.GetValues(typeof(Units.SpeedUnits));
            foreach (var enumVal in speedValues)
            {
                var speedUnit = (Units.SpeedUnits)enumVal;
                switch (speedUnit)
                {
                    default:
                    case Units.SpeedUnits.MilesPerHour:
                        SpeedUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_mph,
                            Tag = speedUnit.GetStringValue()
                        });
                        break;
                    case Units.SpeedUnits.KilometersPerHour:
                        SpeedUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_kph,
                            Tag = speedUnit.GetStringValue()
                        });
                        break;
                    case Units.SpeedUnits.MetersPerSecond:
                        SpeedUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_msec,
                            Tag = speedUnit.GetStringValue()
                        });
                        break;
                    case Units.SpeedUnits.Knots:
                        SpeedUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_knots,
                            Tag = speedUnit.GetStringValue()
                        });
                        break;
                }
            }

            var distanceValues = Enum.GetValues(typeof(Units.DistanceUnits));
            foreach (var enumVal in distanceValues)
            {
                var distanceUnit = (Units.DistanceUnits)enumVal;
                switch (distanceUnit)
                {
                    default:
                    case Units.DistanceUnits.Miles:
                        DistanceUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_miles,
                            Tag = distanceUnit.GetStringValue()
                        });
                        break;
                    case Units.DistanceUnits.Kilometers:
                        DistanceUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_kilometers,
                            Tag = distanceUnit.GetStringValue()
                        });
                        break;
                }
            }

            var precipitationValues = Enum.GetValues(typeof(Units.PrecipitationUnits));
            foreach (var enumVal in precipitationValues)
            {
                var precipitationUnit = (Units.PrecipitationUnits)enumVal;
                switch (precipitationUnit)
                {
                    default:
                    case Units.PrecipitationUnits.Inches:
                        PrecipitationUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_in,
                            Tag = precipitationUnit.GetStringValue()
                        });
                        break;
                    case Units.PrecipitationUnits.Millimeters:
                        PrecipitationUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_mm,
                            Tag = precipitationUnit.GetStringValue()
                        });
                        break;
                }
            }

            var pressureValues = Enum.GetValues(typeof(Units.PressureUnits));
            foreach (var enumVal in pressureValues)
            {
                var pressureUnit = (Units.PressureUnits)enumVal;
                switch (pressureUnit)
                {
                    default:
                    case Units.PressureUnits.InHg:
                        PressureUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_inHg,
                            Tag = pressureUnit.GetStringValue()
                        });
                        break;
                    case Units.PressureUnits.Millibar:
                        PressureUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_mBar,
                            Tag = pressureUnit.GetStringValue()
                        });
                        break;
                    case Units.PressureUnits.MmHg:
                        PressureUnits.Items.Add(new SegmentedItem()
                        {
                            Content = ResUnits.unit_mmHg,
                            Tag = pressureUnit.GetStringValue()
                        });
                        break;
                }
            }

            ResetTitle.Text = App.Current.ResLoader.GetString("pref_title_resetunits");
            ResetImperialUnits.Content = App.Current.ResLoader.GetString("default_units_imperial");
            ResetMetricUnits.Content = App.Current.ResLoader.GetString("default_units_metric");
        }

        private void RegisterListeners()
        {
            TemperatureUnits.SelectionChanged += (s, e) =>
            {
                if (s is Segmented segm && segm.SelectedItem is SegmentedItem item)
                {
                    SettingsManager.TemperatureUnit = item.Tag.ToString();
                    UnitsChanged = true;
                }
            };
            SpeedUnits.SelectionChanged += (s, e) =>
            {
                if (s is Segmented segm && segm.SelectedItem is SegmentedItem item)
                {
                    SettingsManager.SpeedUnit = item.Tag.ToString();
                    UnitsChanged = true;
                }
            };
            DistanceUnits.SelectionChanged += (s, e) =>
            {
                if (s is Segmented segm && segm.SelectedItem is SegmentedItem item)
                {
                    SettingsManager.DistanceUnit = item.Tag.ToString();
                    UnitsChanged = true;
                }
            };
            PrecipitationUnits.SelectionChanged += (s, e) =>
            {
                if (s is Segmented segm && segm.SelectedItem is SegmentedItem item)
                {
                    SettingsManager.PrecipitationUnit = item.Tag.ToString();
                    UnitsChanged = true;
                }
            };
            PressureUnits.SelectionChanged += (s, e) =>
            {
                if (s is Segmented segm && segm.SelectedItem is SegmentedItem item)
                {
                    SettingsManager.PressureUnit = item.Tag.ToString();
                    UnitsChanged = true;
                }
            };
        }

        private void RestoreSettings()
        {
            TemperatureUnits.SelectedItem = TemperatureUnits.Items.FirstOrDefault(btn => string.Equals(SettingsManager.TemperatureUnit, (btn as SegmentedItem).Tag?.ToString()));
            SpeedUnits.SelectedItem = SpeedUnits.Items.FirstOrDefault(btn => string.Equals(SettingsManager.SpeedUnit, (btn as SegmentedItem).Tag?.ToString()));
            DistanceUnits.SelectedItem = DistanceUnits.Items.FirstOrDefault(btn => string.Equals(SettingsManager.DistanceUnit, (btn as SegmentedItem).Tag?.ToString()));
            PrecipitationUnits.SelectedItem = PrecipitationUnits.Items.FirstOrDefault(btn => string.Equals(SettingsManager.PrecipitationUnit, (btn as SegmentedItem).Tag?.ToString()));
            PressureUnits.SelectedItem = PressureUnits.Items.FirstOrDefault(btn => string.Equals(SettingsManager.PressureUnit, (btn as SegmentedItem).Tag?.ToString()));
        }

        private void ResetImperialUnits_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetDefaultUnits(Units.FAHRENHEIT);
            RestoreSettings();
        }

        private void ResetMetricUnits_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.SetDefaultUnits(Units.CELSIUS);
            RestoreSettings();
        }
    }
}
