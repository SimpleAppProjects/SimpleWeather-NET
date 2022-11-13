using SimpleWeather.Common.Controls;
using SimpleWeather.Icons;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class WeatherDetailPanel : UserControl
    {
        private WeatherDetailViewModel ViewModel { get; set; }

        private readonly WeatherIconsManager wim;

        public bool UseMonochrome
        {
            get { return (bool)GetValue(UseMonochromeProperty); }
            set { SetValue(UseMonochromeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseMonochrome.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseMonochromeProperty =
            DependencyProperty.Register("UseMonochrome", typeof(bool), typeof(WeatherDetailPanel), new PropertyMetadata(false));

        public WeatherDetailPanel()
        {
            this.InitializeComponent();
            wim = SharedModule.Instance.WeatherIconsManager;
            ViewModel = new WeatherDetailViewModel();
            this.DataContextChanged += (sender, args) =>
            {
                if (args.NewValue is HourlyForecastItemViewModel)
                    ViewModel.SetForecast(args.NewValue as HourlyForecastItemViewModel);
                else if (args.NewValue is ForecastItemViewModel)
                    ViewModel.SetForecast(args.NewValue as ForecastItemViewModel);

                this.Bindings.Update();
                UseMonochrome = wim.ShouldUseMonochrome();
            };
            this.SizeChanged += (sender, args) =>
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
                    if ((args.NewSize.Width - 50 - IconBox.Margin.Left - IconBox.Margin.Right) <= txtblk.DesiredSize.Width)
                        ConditionDescription.Visibility = Visibility.Visible;
                    else
                        ConditionDescription.Visibility = Visibility.Collapsed;
                }
            };
        }

        internal class WeatherDetailViewModel
        {
            public string Date { get; set; }
            public string Icon { get; set; }
            public string Condition { get; set; }
            public string ConditionLongDesc { get; set; }
            public string PoPChance { get; set; }
            public string Cloudiness { get; set; }
            public string WindSpeed { get; set; }
            public ObservableCollection<DetailItemViewModel> Extras { get; set; }
            public bool HasExtras { get { return Extras?.Count > 0 || !String.IsNullOrWhiteSpace(ConditionLongDesc); } }

            internal WeatherDetailViewModel()
            {
            }

            public void SetForecast(ForecastItemViewModel forecastViewModel)
            {
                Date = forecastViewModel.Date;
                Icon = forecastViewModel.WeatherIcon;
                Condition = String.Format(CultureInfo.InvariantCulture, "{0} | {1} - {2}",
                    forecastViewModel.HiTemp, forecastViewModel.LoTemp, forecastViewModel.Condition);
                ConditionLongDesc = forecastViewModel.ConditionLong;
                Extras = new ObservableCollection<DetailItemViewModel>();

                PoPChance = Cloudiness = WindSpeed = null;

                foreach (DetailItemViewModel detailItem in forecastViewModel.DetailExtras.Values)
                {
                    if (detailItem.DetailsType == WeatherDetailsType.PoPChance)
                    {
                        PoPChance = detailItem.Value;
                        continue;
                    }
                    else if (detailItem.DetailsType == WeatherDetailsType.PoPCloudiness)
                    {
                        Cloudiness = detailItem.Value;
                        continue;
                    }
                    else if (detailItem.DetailsType == WeatherDetailsType.WindSpeed)
                    {
                        WindSpeed = detailItem.Value;
                        continue;
                    }

                    Extras.Add(detailItem);
                }
            }

            public void SetForecast(HourlyForecastItemViewModel hrforecastViewModel)
            {
                Date = hrforecastViewModel.Date;
                Icon = hrforecastViewModel.WeatherIcon;
                Condition = String.Format(CultureInfo.InvariantCulture, "{0} - {1}",
                    hrforecastViewModel.HiTemp, hrforecastViewModel.Condition);
                Extras = new ObservableCollection<DetailItemViewModel>();

                PoPChance = Cloudiness = WindSpeed = null;

                foreach (DetailItemViewModel detailItem in hrforecastViewModel.DetailExtras.Values)
                {
                    if (detailItem.DetailsType == WeatherDetailsType.PoPChance)
                    {
                        PoPChance = detailItem.Value;
                        continue;
                    }
                    else if (detailItem.DetailsType == WeatherDetailsType.PoPCloudiness)
                    {
                        Cloudiness = detailItem.Value;
                        continue;
                    }
                    else if (detailItem.DetailsType == WeatherDetailsType.WindSpeed)
                    {
                        WindSpeed = detailItem.Value;
                        continue;
                    }

                    Extras.Add(detailItem);
                }
            }
        }
    }
}