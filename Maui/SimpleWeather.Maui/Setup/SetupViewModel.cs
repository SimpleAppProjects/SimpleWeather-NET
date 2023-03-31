using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Location;
using SimpleWeather.Maui.Main;
using SimpleWeather.Preferences;

namespace SimpleWeather.Maui.Setup
{
    public partial class SetupViewModel : BaseViewModel, ISetupNavigator
    {
        [ObservableProperty]
        private LocationData.LocationData locationData = null;

        [ObservableProperty]
        private bool isBackButtonVisible = false;
        [ObservableProperty]
        private bool isNavBarBackButtonVisible = false;
        [ObservableProperty]
        private bool isNavBarNextButtonVisible = true;
        [ObservableProperty]
        private int selectedIndex;
        [ObservableProperty]
        private int itemCount;

        private bool isWeatherLoaded = false;

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        public SetupViewModel()
        {
            Init();
        }

        private int _itemCount
        {
            get
            {
                if (isWeatherLoaded)
                    return 2;
                else
                    return 3;
            }
        }

        private int GetPosition(Type destinationType)
        {
            if (destinationType == typeof(SetupWelcomePage))
            {
                return 0;
            }
            else if (destinationType == typeof(SetupLocationsPage) || destinationType == typeof(LocationSearchPage))
            {
                return 1;
            }
            else if (destinationType == typeof(SetupSettingsPage))
            {
                return isWeatherLoaded ? 1 : 2;
            }
            else
            {
                return 0;
            }
        }

        private Type GetNextDestination(Type destinationType)
        {
            if (destinationType == typeof(SetupWelcomePage))
            {
                return isWeatherLoaded ? typeof(SetupSettingsPage) : typeof(SetupLocationsPage);
            }
            else if (destinationType == typeof(SetupLocationsPage) || destinationType == typeof(LocationSearchPage))
            {
                return typeof(SetupSettingsPage);
            }
            else if (destinationType == typeof(SetupSettingsPage))
            {
                return null;
            }
            else
            {
                return isWeatherLoaded ? typeof(SetupSettingsPage) : typeof(SetupLocationsPage);
            }
        }

        private void Init()
        {
            // Setup Pages & Indicator
            isWeatherLoaded = SettingsManager.WeatherLoaded;

            if (isWeatherLoaded && LocationData == null)
            {
                Task.Run(async () =>
                {
                    LocationData = await SettingsManager.GetHomeData();
                });
            }

            ItemCount = _itemCount;
        }

        public async void Back()
        {
            await App.Current.Navigation.PopAsync(true);
        }

        public async void Next()
        {
            if (App.Current.CurrentPage is not IPageVerification page || page.CanContinue())
            {
                var destination = App.Current.CurrentPage.GetType();
                var destinationIdx = GetPosition(destination);
                if (destinationIdx >= _itemCount - 1)
                {
                    // Complete
                    OnCompleted();
                }
                else
                {
                    var nextDestination = GetNextDestination(destination);

                    if (nextDestination != destination)
                    {
                        await App.Current.Navigation.PushAsync(Ioc.Default.GetService(nextDestination) as Page, true);
                    }
                }
            }
        }

        private void OnCompleted()
        {
            SettingsManager.WeatherLoaded = true;
            SettingsManager.OnBoardComplete = true;
            App.Current.MainPage = new AppShell();
        }

        public void OnNavigated()
        {
            if (App.Current.CurrentPage?.GetType() is Type currentPageType)
            {
                // Change indicators
                ItemCount = _itemCount;
                var pageIdx = GetPosition(currentPageType);

                // Update BottomNavBar for registered pages (ignore modals)
                if (pageIdx != -1)
                {
                    SelectedIndex = pageIdx;

                    if (currentPageType == typeof(SetupLocationsPage) || currentPageType == typeof(LocationSearchPage))
                    {
                        IsNavBarBackButtonVisible = true;
                        IsNavBarNextButtonVisible = false;
                    }
                    else if (currentPageType == typeof(SetupSettingsPage))
                    {
                        IsNavBarBackButtonVisible = false;
                        IsNavBarNextButtonVisible = true;
                    }
                    else if (currentPageType == typeof(SetupWelcomePage))
                    {
                        IsNavBarBackButtonVisible = false;
                        IsNavBarNextButtonVisible = true;
                    }
                    else
                    {
                        IsNavBarBackButtonVisible = true;
                        IsNavBarNextButtonVisible = true;
                    }
                }
                IsBackButtonVisible = pageIdx == -1;
            }
        }
    }
}
