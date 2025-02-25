using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
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

    public MoonPhaseViewModel ViewModel => this.BindingContext as MoonPhaseViewModel;

    public MoonPhaseControl()
    {
        this.InitializeComponent();

        DataSet = Enumerable.Repeat(_moonPhaseTypes, 3).SelectMany(phase => phase).Select(p => new MoonPhaseItem(p)).ToList();
        MoonStack.ItemsSource = DataSet;

        this.BindingContextChanged += (sender, args) =>
        {
            ApplyBindings();

            if (ViewModel != null)
            {
                if (SelectedMoonPhaseType != ViewModel.PhaseType)
                {
                    SelectedMoonPhaseType = ViewModel.PhaseType;
                    DataSet.ForEachIndexed((index, item) =>
                    {
                        if (item.MoonPhaseType == SelectedMoonPhaseType)
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
                    });
                }

                ScrollToSelectedPhase();

                MoonriseIcon.UpdateWeatherIcon();
                MoonsetIcon.UpdateWeatherIcon();
            }
        };
        MoonStack.SizeChanged += (sender, args) =>
        {
            Dispatcher.Dispatch(ScrollToSelectedPhase);
        };
    }

    private void ScrollToSelectedPhase()
    {
        MoonStack?.ScrollTo(SelectedIndex, position: ScrollToPosition.Center);
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
        get { return (double)GetValue(OpacityProperty); }
        set { SetValue(OpacityProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Opacity.  This enables animation, styling, binding, etc...
    public static readonly BindableProperty OpacityProperty =
        BindableProperty.Create(nameof(Opacity), typeof(double), typeof(MoonPhaseItem), defaultValue: 1.0d);
}

public static class MoonPhaseControlBindings
{
    public static string MoonPhaseTypeToIcon(MoonPhase.MoonPhaseType moonPhaseType)
    {
        return new DetailItemViewModel(moonPhaseType).Icon;
    }
}