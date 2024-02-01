using CommunityToolkit.Maui.Markup;
using SimpleWeather.Maui.Helpers;
using SimpleWeather.Maui.Stepper;

namespace SimpleWeather.Maui.Setup
{
    public abstract class BaseSetupPage : ContentPage
    {
        public SetupViewModel ViewModel { get; }

        public BottomStepperNavigationBar BottomNavBar;

        public BaseSetupPage(SetupViewModel viewModel)
        {
            this.ViewModel = viewModel;
            BindingContext = viewModel;
            if (App.Current.Resources.TryGetValue("SetupPageTemplate", out var _controlTemplate))
            {
                ControlTemplate = _controlTemplate as ControlTemplate;
            }
            this.Loaded += BaseSetupPage_Loaded;
            this.Unloaded += BaseSetupPage_Unloaded;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BottomNavBar = GetTemplateChild("BottomNavBar") as BottomStepperNavigationBar;
        }

        private void BindNavBar(bool bind)
        {
            if (BottomNavBar != null)
            {
                if (bind)
                {
                    BottomNavBar.OnBackButtonClicked += BottomNavBar_OnBackButtonClicked;
                    BottomNavBar.OnNextButtonClicked += BottomNavBar_OnNextButtonClicked;

                    BottomNavBar.ItemCount = ViewModel.ItemCount;
                    BottomNavBar.SelectedIndex = ViewModel.SelectedIndex;
                }
                else
                {
                    BottomNavBar.OnBackButtonClicked -= BottomNavBar_OnBackButtonClicked;
                    BottomNavBar.OnNextButtonClicked -= BottomNavBar_OnNextButtonClicked;
                }
            }
        }

        private void BottomNavBar_OnBackButtonClicked(object sender, EventArgs e)
        {
            ViewModel.Back();
        }

        private void BottomNavBar_OnNextButtonClicked(object sender, EventArgs e)
        {
            ViewModel.Next();
        }

        private void BaseSetupPage_Loaded(object sender, EventArgs e)
        {
            BindNavBar(true);
        }

        private void BaseSetupPage_Unloaded(object sender, EventArgs e)
        {
            BindNavBar(false);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            ViewModel.OnNavigated();
            UpdateBottomNavBarState();
        }

        private void UpdateBottomNavBarState()
        {
            if (BottomNavBar != null)
            {
                BottomNavBar.SelectedIndex = ViewModel.SelectedIndex;
                BottomNavBar.ShowBackButton(ViewModel.IsNavBarBackButtonVisible);
                BottomNavBar.ShowNextButton(ViewModel.IsNavBarNextButtonVisible);
            }
        }
    }
}