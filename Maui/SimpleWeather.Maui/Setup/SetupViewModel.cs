using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using SimpleWeather.ComponentModel;
using SimpleWeather.Maui.Helpers;
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

        private readonly SettingsManager SettingsManager = Ioc.Default.GetService<SettingsManager>();

        private readonly List<Type> Pages = new List<Type>()
        {
            typeof(SetupWelcomePage),
            typeof(SetupLocationsPage),
            typeof(SetupSettingsPage)
        };

        public SetupViewModel()
        {
            Init();
        }

        private void Init()
        {
            // Setup Pages & Indicator
            if (SettingsManager.WeatherLoaded)
            {
                this.Pages.Remove(typeof(SetupLocationsPage));
            }
            ItemCount = this.Pages.Count;
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
                var destinationIdx = Pages.IndexOf(destination);
                if (destinationIdx >= Pages.Count - 1)
                {
                    // Complete
                    OnCompleted();
                }
                else
                {
                    var nextPage = Pages[destinationIdx + 1];

                    if (nextPage != destination)
                    {
                        await App.Current.Navigation.PushAsync(Ioc.Default.GetService(nextPage) as Page, true);
                    }
                }
            }
        }

        private void OnCompleted()
        {
            SettingsManager.WeatherLoaded = true;
            SettingsManager.OnBoardComplete = true;
            App.Current.MainPage = new AppShell();
            //this.Frame.Navigate(typeof(Shell), ViewModel.LocationData);
        }

        public void OnNavigated()
        {
            if (App.Current.CurrentPage?.GetType() is Type currentPageType)
            {
                // Change indicators
                var pageIdx = Pages.IndexOf(currentPageType);

                // Update BottomNavBar for registered pages (ignore modals)
                if (pageIdx != -1)
                {
                    SelectedIndex = Pages.IndexOf(currentPageType);

                    if (currentPageType == Pages.Last())
                    {
                        IsNavBarBackButtonVisible = false;
                        IsNavBarNextButtonVisible = true;
                    }
                    else if (currentPageType == Pages.First())
                    {
                        IsNavBarBackButtonVisible = false;
                        IsNavBarNextButtonVisible = true;
                    }
                    else if (currentPageType == typeof(SetupLocationsPage))
                    {
                        IsNavBarBackButtonVisible = true;
                        IsNavBarNextButtonVisible = false;
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
