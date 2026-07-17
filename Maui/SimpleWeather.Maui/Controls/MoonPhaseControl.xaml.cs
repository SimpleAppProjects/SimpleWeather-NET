using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Data;

namespace SimpleWeather.Maui.Controls;

public partial class MoonPhaseControl : ContentView
{
    private readonly MoonPhase.MoonPhaseType[] _moonPhaseTypes = Enum.GetValues<MoonPhase.MoonPhaseType>();
    private readonly List<MoonPhaseItem> DataSet;

    private MoonPhase.MoonPhaseType SelectedMoonPhaseType { get; set; } = MoonPhase.MoonPhaseType.FullMoon;
    private int SelectedIndex => (int)SelectedMoonPhaseType + _moonPhaseTypes.Length;

    private bool Initialized = false;

    public MoonPhaseViewModel ViewModel => this.BindingContext as MoonPhaseViewModel;

    public MoonPhaseControl()
    {
        this.InitializeComponent();

        DataSet = Enumerable.Repeat(_moonPhaseTypes, 3).SelectMany(phase => phase).Select(p => new MoonPhaseItem(p)).ToList();
        BindableLayout.SetItemsSource(MoonStack, DataSet);

        MoonStack.SizeChanged += (_, _) =>
        {
            Dispatcher.Dispatch(ScrollToSelectedPhase);
        };
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        ApplyBindings();

        if (ViewModel != null)
        {
            if (SelectedMoonPhaseType != ViewModel.PhaseType || !Initialized)
            {
                SelectedMoonPhaseType = ViewModel.PhaseType;
                DataSet.ForEachIndexed((index, item) =>
                {
                    var isSelected = item.MoonPhaseType == SelectedMoonPhaseType;

                    if (isSelected)
                    {
                        item.Opacity = 1.0;
                    }
                    else if (index == SelectedIndex - 2 || index == SelectedIndex + 2)
                    {
                        item.Opacity = 0.2;
                    }
                    else if (index == SelectedIndex - 3 || index == SelectedIndex + 3)
                    {
                        item.Opacity = 0.15;
                    }
                    else
                    {
                        item.Opacity = 0.35;
                    }

                    item.ShouldAnimate = isSelected;
                });

                Initialized = true;
            }

            ScrollToSelectedPhase();

            MoonriseIcon.UpdateWeatherIcon();
            MoonsetIcon.UpdateWeatherIcon();
        }
    }

    private void ScrollToSelectedPhase()
    {
        MoonStack?.Cast<IconControl>()?.FirstOrDefault()?.Let(v =>
        {
            Dispatcher.Dispatch(async () =>
            {
                await MoonStackScroller.ScrollToAsync(((v.IconWidth + 8) * SelectedIndex) - MoonStackScroller.Width / 2 +
                                                      ((v.IconWidth + 8) / 2), 0, true);
            });
        });
    }

    private void IconControl_Loaded(object sender, EventArgs e)
    {
        (sender as IconControl)?.UpdateWeatherIcon();
    }
}

public class MoonPhaseItem(MoonPhase.MoonPhaseType moonPhaseType) : BindableObject
{
    public MoonPhase.MoonPhaseType MoonPhaseType { get; set; } = moonPhaseType;
    public string Icon { get; set; } = MoonPhaseControlBindings.MoonPhaseTypeToIcon(moonPhaseType);

    public double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    // Using a DependencyProperty as the backing store for Opacity.  This enables animation, styling, binding, etc...
    public static readonly BindableProperty OpacityProperty =
        BindableProperty.Create(nameof(Opacity), typeof(double), typeof(MoonPhaseItem), defaultValue: 1.0d);

    public bool ShouldAnimate
    {
        get { return (bool)GetValue(ShouldAnimateProperty); }
        set { SetValue(ShouldAnimateProperty, value); }
    }

    // Using a BindableProperty as the backing store for ShouldAnimate.  This enables animation, styling, binding, etc...
    public static readonly BindableProperty ShouldAnimateProperty =
        BindableProperty.Create(nameof(ShouldAnimate), typeof(bool), typeof(MoonPhaseItem), defaultValue: false);
}

public static class MoonPhaseControlBindings
{
    public static string MoonPhaseTypeToIcon(MoonPhase.MoonPhaseType moonPhaseType)
    {
        return new DetailItemViewModel(moonPhaseType).Icon;
    }
}