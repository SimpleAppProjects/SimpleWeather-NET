using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SimpleWeather.Common.Controls;
using System.Collections.ObjectModel;
using Windows.Foundation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.NET.Controls
{
    public sealed partial class WeatherDetailPanel : UserControl
    {
        private readonly WeatherDetailViewModel ViewModel;

        public string WeatherIcon
        {
            get { return (string)GetValue(WeatherIconProperty); }
            set { SetValue(WeatherIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeatherIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeatherIconProperty =
            DependencyProperty.Register(nameof(WeatherIcon), typeof(string), typeof(WeatherDetailPanel), new PropertyMetadata(null));

        public WeatherDetailPanel()
        {
            this.InitializeComponent();
            ViewModel = new WeatherDetailViewModel();
            this.DataContextChanged += (sender, args) =>
            {
                if (args.NewValue is HourlyForecastItemViewModel)
                    ViewModel.SetForecast(args.NewValue as HourlyForecastItemViewModel);
                else if (args.NewValue is ForecastItemViewModel)
                    ViewModel.SetForecast(args.NewValue as ForecastItemViewModel);

                this.Bindings.Update();

                DispatcherQueue.TryEnqueue(() =>
                {
                    MeasureConditionDescText(this.ActualWidth);
                });
            };
            this.SizeChanged += (sender, args) =>
            {
                MeasureConditionDescText(args.NewSize.Width);
            };
        }

        private void MeasureConditionDescText(double width)
        {
            var txtblk = new TextBlock()
            {
                Text = ConditionDescription.Text,
                FontSize = ConditionDescription.FontSize
            };
            txtblk.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (!String.IsNullOrWhiteSpace(ViewModel.ConditionLongDesc) && (bool)CondDescFirstRun.Text?.Equals(ViewModel.ConditionLongDesc))
            {
                ConditionDescription.Visibility = Visibility.Visible;
            }
            else
            {
                if ((width - 60 - IconBox.Margin.Left - IconBox.Margin.Right) <= txtblk.DesiredSize.Width)
                    ConditionDescription.Visibility = Visibility.Visible;
                else
                    ConditionDescription.Visibility = Visibility.Collapsed;
            }
        }

        internal class WeatherDetailViewModel
        {
            public string Date { get; set; }
            public string Icon { get; set; }
            public string HiLo { get; set; }
            public string Condition { get; set; }
            public string ConditionLongDesc { get; set; }
            public ObservableCollection<DetailItemViewModel> Extras { get; set; }
            public bool HasExtras { get { return Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc); } }

            internal WeatherDetailViewModel()
            {
            }

            public void SetForecast(ForecastItemViewModel forecastViewModel)
            {
                Date = forecastViewModel.Date;
                Icon = forecastViewModel.WeatherIcon;
                HiLo = $"{forecastViewModel.HiTemp} / {forecastViewModel.LoTemp}";
                Condition = forecastViewModel.Condition;
                ConditionLongDesc = forecastViewModel.ConditionLong;
                Extras = [.. forecastViewModel.DetailExtras.Values];
            }

            public void SetForecast(HourlyForecastItemViewModel hrforecastViewModel)
            {
                Date = hrforecastViewModel.Date;
                Icon = hrforecastViewModel.WeatherIcon;
                HiLo = hrforecastViewModel.HiTemp;
                Condition = hrforecastViewModel.Condition;
                ConditionLongDesc = null;
                Extras = [.. hrforecastViewModel.DetailExtras.Values];
            }
        }
    }
}