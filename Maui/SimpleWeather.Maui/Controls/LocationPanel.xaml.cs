using System.ComponentModel;
using SimpleWeather.NET.Controls;
using SimpleWeather.NET.Utils;

namespace SimpleWeather.Maui.Controls;

public partial class LocationPanel : ContentView
{
    public LocationPanelUiModel ViewModel
    {
        get => this.BindingContext as LocationPanelUiModel;
    }

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

    public event EventHandler LongPress;

    public LocationPanel()
	{
		InitializeComponent();
        this.BindingContextChanged += (s, e) =>
        {
            UpdateControlTheme();
        };
        this.Loaded += LocationPanel_Loaded;
        this.Unloaded += LocationPanel_Unloaded;
	}

    private void LocationPanel_Loaded(object sender, EventArgs e)
    {
        if (this.Handler is IPlatformViewHandler handler && handler.PlatformView is not null)
        {
            RegisterLongPress(handler.PlatformView);
        }
    }

    private void LocationPanel_Unloaded(object sender, EventArgs e)
    {
        if (this.Handler is IPlatformViewHandler handler && handler.PlatformView is not null)
        {
            UnregisterLongPress(handler.PlatformView);
        }
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);
        if (args.OldHandler is IPlatformViewHandler oldHandler && oldHandler.PlatformView is not null)
        {
            UnregisterLongPress(oldHandler.PlatformView);
        }
        if (args.NewHandler is IPlatformViewHandler newHandler && newHandler.PlatformView is not null)
        {
            RegisterLongPress(newHandler.PlatformView);
        }
    }

    private static void OnThemeablePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is LocationPanel panel)
        {
            if (panel.IsLoaded)
            {
                panel.UpdateControlTheme(FeatureSettings.LocationPanelBackgroundImage && panel?.BackgroundOverlay?.Source != null);
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
            BackgroundOverlay?.SetValue(VisualElement.IsVisibleProperty, true);
            GradientOverlay?.SetValue(VisualElement.IsVisibleProperty, true);
            ConditionPanelTextColor = Colors.White;
            ForceIconDarkTheme = true;
        }
        else
        {
            BackgroundOverlay?.SetValue(VisualElement.IsVisibleProperty, false);
            GradientOverlay?.SetValue(VisualElement.IsVisibleProperty, false);
            this.SetAppThemeColor(ConditionPanelTextColorProperty, Colors.Black, Colors.White);
            ForceIconDarkTheme = false;
        }
        ApplyBindings();
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
            UpdateControlTheme(FeatureSettings.LocationPanelBackgroundImage && bgImage.Source != null);
        }
    }

#if WINDOWS
    private void RegisterLongPress(Microsoft.UI.Xaml.FrameworkElement view)
    {

    }
#elif IOS || MACCATALYST
    private void RegisterLongPress(UIKit.UIView view)
    {
        view.UserInteractionEnabled = true;
        longPressGestureRecognizer ??= new UIKit.UILongPressGestureRecognizer(() =>
        {
            LongPress?.Invoke(this, EventArgs.Empty);
        });
        view.AddGestureRecognizer(longPressGestureRecognizer);
    }

    private void UnregisterLongPress(UIKit.UIView view)
    {
        if (longPressGestureRecognizer != null)
        {
            view.RemoveGestureRecognizer(longPressGestureRecognizer);
        }
    }

    private UIKit.UILongPressGestureRecognizer longPressGestureRecognizer;
#elif ANDROID
    private void RegisterLongPress(Android.Views.View view)
    {
        view.LongClickable = true;
        view.LongClick += View_LongClick;
    }

    private void UnregisterLongPress(Android.Views.View view)
    {
        view.LongClickable = false;
        view.LongClick -= View_LongClick;
    }

    private void View_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
    {
        LongPress?.Invoke(this, EventArgs.Empty);
    }
#else
    private void RegisterLongPress(object view)
    {

    }
#endif
}
