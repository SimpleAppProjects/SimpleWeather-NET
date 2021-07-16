using SimpleWeather.Controls;
using SimpleWeather.Icons;
using SimpleWeather.WeatherData;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleWeather.UWP.Controls
{
    public sealed partial class WeatherDetailPanel : UserControl
    {
        private WeatherDetailViewModel ViewModel { get; set; }

        private readonly WeatherIconsManager wim = WeatherIconsManager.GetInstance();

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

                foreach (DetailItemViewModel detailItem in forecastViewModel.DetailExtras)
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

                foreach (DetailItemViewModel detailItem in hrforecastViewModel.DetailExtras)
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