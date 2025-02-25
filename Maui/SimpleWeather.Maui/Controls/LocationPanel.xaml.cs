using System.ComponentModel;
using CommunityToolkit.Maui.Markup;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Utils;

namespace SimpleWeather.Maui.Controls;

public partial class LocationPanel : ContentView
{
    public LocationPanelUiModel ViewModel => this.BindingContext as LocationPanelUiModel;

    public Color ConditionPanelTextColor
    {
        get => (Color)GetValue(ConditionPanelTextColorProperty);
        set => SetValue(ConditionPanelTextColorProperty, value);
    }

    public static readonly BindableProperty ConditionPanelTextColorProperty =
        BindableProperty.Create(nameof(ConditionPanelTextColor), typeof(Color), typeof(LocationPanel), Colors.White, propertyChanged: OnThemeablePropertyChanged);

    public bool ForceIconDarkTheme
    {
        get => (bool)GetValue(ForceIconDarkThemeProperty);
        set => SetValue(ForceIconDarkThemeProperty, value);
    }

    public static readonly BindableProperty ForceIconDarkThemeProperty =
        BindableProperty.Create(nameof(ForceIconDarkTheme), typeof(bool), typeof(LocationPanel), false, propertyChanged: OnThemeablePropertyChanged);

    public LocationPanel()
	{
		InitializeComponent();
        this.BindingContextChanged += (s, e) =>
        {
            UpdateControlTheme();
        };
        UpdateControlTheme();
    }

    private static void OnThemeablePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LocationPanel panel)
        {
            if (panel.BackgroundOverlay is { } bgImage)
            {
                panel.UpdateControlTheme(FeatureSettings.LocationPanelBackgroundImage && bgImage.Source != null);
            }
            else
            {
                panel.UpdateControlTheme();
            }
        }
    }

    private void UpdateControlTheme()
    {
        UpdateControlTheme(FeatureSettings.LocationPanelBackgroundImage);
    }

    private void UpdateControlTheme(bool backgroundEnabled)
    {
        if (backgroundEnabled)
        {
            BackgroundOverlay?.IsVisible(true);
            GradientOverlay?.IsVisible(true);
            ConditionPanelTextColor = Colors.White;
            ForceIconDarkTheme = true;
        }
        else
        {
            BackgroundOverlay?.IsVisible(false);
            GradientOverlay?.IsVisible(false);
            this.SetAppThemeColor(ConditionPanelTextColorProperty, Colors.Black, Colors.White);
            ForceIconDarkTheme = false;
        }
        Dispatcher.Dispatch(ApplyBindings);
    }

    private void BackgroundOverlay_Loaded(object sender, EventArgs e)
    {
        var bgImage = sender as Image;
        bgImage.PropertyChanged += BgImage_PropertyChanged;
        UpdateControlTheme(FeatureSettings.LocationPanelBackgroundImage && bgImage.Source != null);
    }

    private void BackgroundOverlay_Unloaded(object sender, EventArgs e)
    {
        var bgImage = sender as Image;
        bgImage.PropertyChanged -= BgImage_PropertyChanged;
    }

    private void BgImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var bgImage = sender as Image;
        if (e.PropertyName == nameof(bgImage.Source))
        {
            UpdateControlTheme(FeatureSettings.LocationPanelBackgroundImage && bgImage?.Source != null);
        }
    }
}
