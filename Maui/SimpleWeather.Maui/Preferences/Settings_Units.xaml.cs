using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using Plugin.Maui.SegmentedControl;
using SimpleWeather.Controls;
using SimpleWeather.Maui.Controls;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using System;
using System.Linq;
using ResStrings = SimpleWeather.Resources.Strings.Resources;
using ResUnits = SimpleWeather.Resources.Strings.Units;
using SegmentValueChangedEventArgs = Plugin.Maui.SegmentedControl.ValueChangedEventArgs;

namespace SimpleWeather.Maui.Preferences;

public partial class Settings_Units : ContentPage
{
    private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

    private bool UnitsChanged = false;

    public Settings_Units()
    {
        InitializeComponent();
        AddUnits();
        RestoreSettings();
        RegisterListeners();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        AnalyticsLogger.LogEvent("Settings_Units: OnNavigatedToPage");
        UnitsChanged = false;
        RestoreSettings();
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);

        // Trigger background task if necessary
        if (UnitsChanged)
        {
            SharedModule.Instance.RequestAction(CommonActions.ACTION_SETTINGS_UPDATEUNIT);
            UnitsChanged = false;
        }
    }

    private void AddUnits()
    {
        TemperatureUnits.Children = Enum.GetValues(typeof(Units.TemperatureUnits))
            .OfType<Units.TemperatureUnits>()
            .Select(tempUnit =>
            {
                return tempUnit switch
                {
                    Units.TemperatureUnits.Celsuis => new SegmentedControlItem("°C", tempUnit.GetStringValue()),
                    _ => new SegmentedControlItem("°F", tempUnit.GetStringValue()),
                };
            }).ToList<SegmentedControlOption>();

        SpeedUnits.Children = Enum.GetValues(typeof(Units.SpeedUnits))
            .OfType<Units.SpeedUnits>()
            .Select(speedUnit =>
            {
                return speedUnit switch
                {
                    Units.SpeedUnits.KilometersPerHour => new SegmentedControlItem(ResUnits.unit_kph, speedUnit.GetStringValue()),
                    Units.SpeedUnits.MetersPerSecond => new SegmentedControlItem(ResUnits.unit_msec, speedUnit.GetStringValue()),
                    Units.SpeedUnits.Knots => new SegmentedControlItem(ResUnits.unit_knots, speedUnit.GetStringValue()),
                    _ => new SegmentedControlItem(ResUnits.unit_mph, speedUnit.GetStringValue()),
                };
            }).ToList<SegmentedControlOption>();

        DistanceUnits.Children = Enum.GetValues(typeof(Units.DistanceUnits))
            .OfType<Units.DistanceUnits>()
            .Select(distanceUnit =>
            {
                return distanceUnit switch
                {
                    Units.DistanceUnits.Kilometers => new SegmentedControlItem(ResUnits.unit_kilometers, distanceUnit.GetStringValue()),
                    _ => new SegmentedControlItem(ResUnits.unit_miles, distanceUnit.GetStringValue()),
                };
            }).ToList<SegmentedControlOption>();

        PrecipitationUnits.Children = Enum.GetValues(typeof(Units.PrecipitationUnits))
            .OfType<Units.PrecipitationUnits>()
            .Select(precipitationUnit =>
            {
                return precipitationUnit switch
                {
                    Units.PrecipitationUnits.Millimeters => new SegmentedControlItem(ResUnits.unit_mm, precipitationUnit.GetStringValue()),
                    _ => new SegmentedControlItem(ResUnits.unit_in, precipitationUnit.GetStringValue()),
                };
            }).ToList<SegmentedControlOption>();

        PressureUnits.Children = Enum.GetValues(typeof(Units.PressureUnits))
            .OfType<Units.PressureUnits>()
            .Select(pressureUnit =>
            {
                return pressureUnit switch
                {
                    Units.PressureUnits.Millibar => new SegmentedControlItem(ResUnits.unit_mBar, pressureUnit.GetStringValue()),
                    Units.PressureUnits.MmHg => new SegmentedControlItem(ResUnits.unit_mmHg, pressureUnit.GetStringValue()),
                    _ => new SegmentedControlItem(ResUnits.unit_inHg, pressureUnit.GetStringValue()),
                };
            }).ToList<SegmentedControlOption>();

        ResetTitle.Text = ResStrings.pref_title_resetunits;
        ResetImperialUnits.Text = ResStrings.default_units_imperial;
        ResetMetricUnits.Text = ResStrings.default_units_metric;
    }

    private void RegisterListeners()
    {
        TemperatureUnits.ValueChanged += TemperatureUnits_SelectionChanged;
        SpeedUnits.ValueChanged += SpeedUnits_SelectionChanged;
        DistanceUnits.ValueChanged += DistanceUnits_SelectionChanged;
        PrecipitationUnits.ValueChanged += PrecipitationUnits_SelectionChanged;
        PressureUnits.ValueChanged += PressureUnits_SelectionChanged;
    }

    private void TemperatureUnits_SelectionChanged(object sender, SegmentValueChangedEventArgs e)
    {
        if (sender is SegmentedControl box && box.SelectedSegment != -1)
        {
            var value = box.Children?.OfType<SegmentedControlItem>()?.ElementAt(box.SelectedSegment);

            if (!string.IsNullOrWhiteSpace(value?.Value?.ToString()))
            {
                SettingsManager.TemperatureUnit = value.Value.ToString();
                UnitsChanged = true;
            }
        }
    }

    private void SpeedUnits_SelectionChanged(object sender, SegmentValueChangedEventArgs e)
    {
        if (sender is SegmentedControl box && box.SelectedSegment != -1)
        {
            var value = box.Children?.OfType<SegmentedControlItem>()?.ElementAt(box.SelectedSegment);

            if (!string.IsNullOrWhiteSpace(value?.Value?.ToString()))
            {
                SettingsManager.SpeedUnit = value.Value.ToString();
                UnitsChanged = true;
            }
        }
    }

    private void DistanceUnits_SelectionChanged(object sender, SegmentValueChangedEventArgs e)
    {
        if (sender is SegmentedControl box && box.SelectedSegment != -1)
        {
            var value = box.Children?.OfType<SegmentedControlItem>()?.ElementAt(box.SelectedSegment);

            if (!string.IsNullOrWhiteSpace(value?.Value?.ToString()))
            {
                SettingsManager.DistanceUnit = value.Value.ToString();
                UnitsChanged = true;
            }
        }
    }

    private void PrecipitationUnits_SelectionChanged(object sender, SegmentValueChangedEventArgs e)
    {
        if (sender is SegmentedControl box && box.SelectedSegment != -1)
        {
            var value = box.Children?.OfType<SegmentedControlItem>()?.ElementAt(box.SelectedSegment);

            if (!string.IsNullOrWhiteSpace(value?.Value?.ToString()))
            {
                SettingsManager.PrecipitationUnit = value.Value.ToString();
                UnitsChanged = true;
            }
        }
    }

    private void PressureUnits_SelectionChanged(object sender, SegmentValueChangedEventArgs e)
    {
        if (sender is SegmentedControl box && box.SelectedSegment != -1)
        {
            var value = box.Children?.OfType<SegmentedControlItem>()?.ElementAt(box.SelectedSegment);

            if (!string.IsNullOrWhiteSpace(value?.Value?.ToString()))
            {
                SettingsManager.PressureUnit = value.Value.ToString();
                UnitsChanged = true;
            }
        }
    }

    private void RestoreSettings()
    {
        TemperatureUnits.SelectedSegment = TemperatureUnits.Children.IndexOf(TemperatureUnits.Children.OfType<SegmentedControlItem>().First(it => Equals(it.Value, SettingsManager.TemperatureUnit)));
        SpeedUnits.SelectedSegment = SpeedUnits.Children.IndexOf(SpeedUnits.Children.OfType<SegmentedControlItem>().First(it => Equals(it.Value, SettingsManager.SpeedUnit)));
        DistanceUnits.SelectedSegment = DistanceUnits.Children.IndexOf(DistanceUnits.Children.OfType<SegmentedControlItem>().First(it => Equals(it.Value, SettingsManager.DistanceUnit)));
        PrecipitationUnits.SelectedSegment = PrecipitationUnits.Children.IndexOf(PrecipitationUnits.Children.OfType<SegmentedControlItem>().First(it => Equals(it.Value, SettingsManager.PrecipitationUnit)));
        PressureUnits.SelectedSegment = PressureUnits.Children.IndexOf(PressureUnits.Children.OfType<SegmentedControlItem>().First(it => Equals(it.Value, SettingsManager.PressureUnit)));
    }

    private void ResetImperialUnits_Clicked(object sender, EventArgs e)
    {
        SettingsManager.SetDefaultUnits(Units.FAHRENHEIT);
        RestoreSettings();
    }

    private void ResetMetricUnits_Clicked(object sender, EventArgs e)
    {
        SettingsManager.SetDefaultUnits(Units.CELSIUS);
        RestoreSettings();
    }
}