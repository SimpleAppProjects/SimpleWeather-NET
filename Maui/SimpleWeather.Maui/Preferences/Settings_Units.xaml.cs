using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.Preferences;
using SimpleWeather.Utils;
using ResStrings = SimpleWeather.Resources.Strings.Resources;

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
        RadioButtonGroup.SetGroupName(TemperatureUnits, nameof(TemperatureUnits));
        var tempValues = Enum.GetValues(typeof(Units.TemperatureUnits));
        foreach (var enumVal in tempValues)
        {
            var tempUnit = (Units.TemperatureUnits)enumVal;
            switch (tempUnit)
            {
                default:
                case Units.TemperatureUnits.Fahrenheit:
                    TemperatureUnits.Children.Add(new RadioButton()
                    {
                        Content = "°F",
                        Value = tempUnit.GetStringValue(),
                        GroupName = nameof(TemperatureUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
                case Units.TemperatureUnits.Celsuis:
                    TemperatureUnits.Children.Add(new RadioButton()
                    {
                        Content = "°C",
                        Value = tempUnit.GetStringValue(),
                        GroupName = nameof(TemperatureUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
            }
        }

        RadioButtonGroup.SetGroupName(SpeedUnits, nameof(SpeedUnits));
        var speedValues = Enum.GetValues(typeof(Units.SpeedUnits));
        foreach (var enumVal in speedValues)
        {
            var speedUnit = (Units.SpeedUnits)enumVal;
            switch (speedUnit)
            {
                default:
                case Units.SpeedUnits.MilesPerHour:
                    SpeedUnits.Children.Add(new RadioButton()
                    {
                        Content = "mph",
                        Value = speedUnit.GetStringValue(),
                        GroupName = nameof(SpeedUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
                case Units.SpeedUnits.KilometersPerHour:
                    SpeedUnits.Children.Add(new RadioButton()
                    {
                        Content = "km/h",
                        Value = speedUnit.GetStringValue(),
                        GroupName = nameof(SpeedUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
                case Units.SpeedUnits.MetersPerSecond:
                    SpeedUnits.Children.Add(new RadioButton()
                    {
                        Content = "m/s",
                        Value = speedUnit.GetStringValue(),
                        GroupName = nameof(SpeedUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
            }
        }

        RadioButtonGroup.SetGroupName(DistanceUnits, nameof(DistanceUnits));
        var distanceValues = Enum.GetValues(typeof(Units.DistanceUnits));
        foreach (var enumVal in distanceValues)
        {
            var distanceUnit = (Units.DistanceUnits)enumVal;
            switch (distanceUnit)
            {
                default:
                case Units.DistanceUnits.Miles:
                    DistanceUnits.Children.Add(new RadioButton()
                    {
                        Content = "mi",
                        Value = distanceUnit.GetStringValue(),
                        GroupName = nameof(DistanceUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
                case Units.DistanceUnits.Kilometers:
                    DistanceUnits.Children.Add(new RadioButton()
                    {
                        Content = "km",
                        Value = distanceUnit.GetStringValue(),
                        GroupName = nameof(DistanceUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
            }
        }

        RadioButtonGroup.SetGroupName(PrecipitationUnits, nameof(PrecipitationUnits));
        var precipitationValues = Enum.GetValues(typeof(Units.PrecipitationUnits));
        foreach (var enumVal in precipitationValues)
        {
            var precipitationUnit = (Units.PrecipitationUnits)enumVal;
            switch (precipitationUnit)
            {
                default:
                case Units.PrecipitationUnits.Inches:
                    PrecipitationUnits.Children.Add(new RadioButton()
                    {
                        Content = "in",
                        Value = precipitationUnit.GetStringValue(),
                        GroupName = nameof(PrecipitationUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
                case Units.PrecipitationUnits.Millimeters:
                    PrecipitationUnits.Children.Add(new RadioButton()
                    {
                        Content = "mm",
                        Value = precipitationUnit.GetStringValue(),
                        GroupName = nameof(PrecipitationUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
            }
        }

        RadioButtonGroup.SetGroupName(PressureUnits, nameof(PressureUnits));
        var pressureValues = Enum.GetValues(typeof(Units.PressureUnits));
        foreach (var enumVal in pressureValues)
        {
            var pressureUnit = (Units.PressureUnits)enumVal;
            switch (pressureUnit)
            {
                default:
                case Units.PressureUnits.InHg:
                    PressureUnits.Children.Add(new RadioButton()
                    {
                        Content = "inHg",
                        Value = pressureUnit.GetStringValue(),
                        GroupName = nameof(PressureUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
                case Units.PressureUnits.Millibar:
                    PressureUnits.Children.Add(new RadioButton()
                    {
                        Content = "mb",
                        Value = pressureUnit.GetStringValue(),
                        GroupName = nameof(PressureUnits),
                        MinimumWidthRequest = 85
                    });
                    break;
            }
        }

        ResetTitle.Text = ResStrings.pref_title_resetunits;
        ResetImperialUnits.Text = ResStrings.default_units_imperial;
        ResetMetricUnits.Text = ResStrings.default_units_metric;
    }

    private void RegisterListeners()
    {
        TemperatureUnits.Children.OfType<RadioButton>().ForEach(btn =>
        {
            btn.CheckedChanged += TemperatureUnits_CheckedChanged;
            btn.TapGesture(() =>
            {
                btn.IsChecked = true;
            });
#if WINDOWS || MACCATALYST
            btn.ClickGesture(() =>
            {
                btn.IsChecked = true;
            });
#endif
        });

        SpeedUnits.Children.OfType<RadioButton>().ForEach(btn =>
        {
            btn.CheckedChanged += SpeedUnits_CheckedChanged;
            btn.TapGesture(() =>
            {
                btn.IsChecked = true;
            });
#if WINDOWS || MACCATALYST
            btn.ClickGesture(() =>
            {
                btn.IsChecked = true;
            });
#endif
        });

        DistanceUnits.Children.OfType<RadioButton>().ForEach(btn =>
        {
            btn.CheckedChanged += DistanceUnits_CheckedChanged;
            btn.TapGesture(() =>
            {
                btn.IsChecked = true;
            });
#if WINDOWS || MACCATALYST
            btn.ClickGesture(() =>
            {
                btn.IsChecked = true;
            });
#endif
        });

        PrecipitationUnits.Children.OfType<RadioButton>().ForEach(btn =>
        {
            btn.CheckedChanged += PrecipitationUnits_CheckedChanged;
            btn.TapGesture(() =>
            {
                btn.IsChecked = true;
            });
#if WINDOWS || MACCATALYST
            btn.ClickGesture(() =>
            {
                btn.IsChecked = true;
            });
#endif
        });

        PressureUnits.Children.OfType<RadioButton>().ForEach(btn =>
        {
            btn.CheckedChanged += PressureUnits_CheckedChanged;
            btn.TapGesture(() =>
            {
                btn.IsChecked = true;
            });
#if WINDOWS || MACCATALYST
            btn.ClickGesture(() =>
            {
                btn.IsChecked = true;
            });
#endif
        });
    }

    private void TemperatureUnits_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton btn && e.Value)
        {
            SettingsManager.TemperatureUnit = btn.Value.ToString();
            UnitsChanged = true;
        }
    }

    private void SpeedUnits_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton btn && e.Value)
        {
            SettingsManager.SpeedUnit = btn.Value.ToString();
            UnitsChanged = true;
        }
    }

    private void DistanceUnits_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton btn && e.Value)
        {
            SettingsManager.DistanceUnit = btn.Value.ToString();
            UnitsChanged = true;
        }
    }

    private void PrecipitationUnits_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton btn && e.Value)
        {
            SettingsManager.PrecipitationUnit = btn.Value.ToString();
            UnitsChanged = true;
        }
    }

    private void PressureUnits_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton btn && e.Value)
        {
            SettingsManager.PressureUnit = btn.Value.ToString();
            UnitsChanged = true;
        }
    }

    private void RestoreSettings()
    {
        RadioButtonGroup.SetSelectedValue(TemperatureUnits, TemperatureUnits.Children.OfType<RadioButton>().FirstOrDefault(btn => string.Equals(SettingsManager.TemperatureUnit, btn.Value?.ToString()))?.Value);
        RadioButtonGroup.SetSelectedValue(SpeedUnits, SpeedUnits.Children.OfType<RadioButton>().FirstOrDefault(btn => string.Equals(SettingsManager.SpeedUnit, btn.Value?.ToString()))?.Value);
        RadioButtonGroup.SetSelectedValue(DistanceUnits, DistanceUnits.Children.OfType<RadioButton>().FirstOrDefault(btn => string.Equals(SettingsManager.DistanceUnit, btn.Value?.ToString()))?.Value);
        RadioButtonGroup.SetSelectedValue(PrecipitationUnits, PrecipitationUnits.Children.OfType<RadioButton>().FirstOrDefault(btn => string.Equals(SettingsManager.PrecipitationUnit, btn.Value?.ToString()))?.Value);
        RadioButtonGroup.SetSelectedValue(PressureUnits, PressureUnits.Children.OfType<RadioButton>().FirstOrDefault(btn => string.Equals(SettingsManager.PressureUnit, btn.Value?.ToString()))?.Value);
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