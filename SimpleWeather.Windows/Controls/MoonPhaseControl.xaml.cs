using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Common.Controls;
using SimpleWeather.Utils;
using SimpleWeather.WeatherData;
using System.Data;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class MoonPhaseControl : UserControl
    {
        private readonly MoonPhase.MoonPhaseType[] _moonPhaseTypes = Enum.GetValues<MoonPhase.MoonPhaseType>();
        private readonly List<MoonPhaseItem> DataSet;

        private MoonPhase.MoonPhaseType SelectedMoonPhaseType { get; set; } = MoonPhase.MoonPhaseType.FullMoon;
        private int SelectedIndex => (int)SelectedMoonPhaseType + _moonPhaseTypes.Length;

        public MoonPhaseViewModel ViewModel
        {
            get { return (this.DataContext as MoonPhaseViewModel); }
        }

        public MoonPhaseControl()
        {
            this.InitializeComponent();

            DataSet = Enumerable.Repeat(_moonPhaseTypes, 3).SelectMany(phase => phase).Select(p => new MoonPhaseItem(p)).ToList();

            this.DataContextChanged += (sender, args) =>
            {
                this.Bindings.Update();

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
                }
            };
            MoonStack.SizeChanged += (sender, args) =>
            {
                ScrollToSelectedPhase();
            };
            MoonStack.RegisterPropertyChangedCallback(ItemsView.ScrollViewProperty, (obj, prop) =>
            {
                if (prop == ItemsView.ScrollViewProperty)
                {
                    var stack = obj as ItemsView;
                    stack.ScrollView.HorizontalScrollBarVisibility = ScrollingScrollBarVisibility.Hidden;
                    stack.ScrollView.VerticalScrollBarVisibility = ScrollingScrollBarVisibility.Hidden;
                }
            });
        }

        private void ScrollToSelectedPhase()
        {
            MoonStack?.ScrollView?.ScrollTo(68 * SelectedIndex - (MoonStack.ActualWidth / 2) + 50, 0);
        }
    }

    public class MoonPhaseItem(MoonPhase.MoonPhaseType moonPhaseType) : DependencyObject
    {
        public MoonPhase.MoonPhaseType MoonPhaseType { get; set; } = moonPhaseType;
        public string Icon { get; set; } = MoonPhaseControlBindings.MoonPhaseTypeToIcon(moonPhaseType);

        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Opacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(MoonPhaseItem), new PropertyMetadata(1.0));
    }

    public static class MoonPhaseControlBindings
    {
        public static string MoonPhaseTypeToIcon(MoonPhase.MoonPhaseType moonPhaseType)
        {
            return new DetailItemViewModel(moonPhaseType).Icon;
        }
    }
}
