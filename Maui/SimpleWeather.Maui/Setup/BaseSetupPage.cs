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
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BottomNavBar = GetTemplateChild("BottomNavBar") as BottomStepperNavigationBar;
            BindNavBar(true);
        }

        private void BindNavBar(bool bind)
        {
            if (BottomNavBar != null)
            {
                if (bind)
                {
                    BottomNavBar.OnBackButtonClicked += BottomNavBar_OnBackButtonClicked;
                    BottomNavBar.OnNextButtonClicked += BottomNavBar_OnNextButtonClicked;
                    BottomNavBar.Unloaded += BottomNavBar_Unloaded;
                    ViewModel.PropertyChanged += ViewModel_PropertyChanged;

                    BottomNavBar.ItemCount = ViewModel.ItemCount;
                    BottomNavBar.SelectedIndex = ViewModel.SelectedIndex;
                }
                else
                {
                    BottomNavBar.OnBackButtonClicked -= BottomNavBar_OnBackButtonClicked;
                    BottomNavBar.OnNextButtonClicked -= BottomNavBar_OnNextButtonClicked;
                    BottomNavBar.Unloaded -= BottomNavBar_Unloaded;
                    ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsNavBarNextButtonVisible))
            {
                BottomNavBar?.ShowNextButton(ViewModel.IsNavBarNextButtonVisible);
            }
            else if (e.PropertyName == nameof(ViewModel.IsNavBarBackButtonVisible))
            {
                BottomNavBar?.ShowBackButton(ViewModel.IsNavBarBackButtonVisible);
            }
            else if (e.PropertyName == nameof(ViewModel.ItemCount))
            {
                BottomNavBar.ItemCount = ViewModel.ItemCount;
            }
            else if (e.PropertyName == nameof(ViewModel.SelectedIndex))
            {
                BottomNavBar.SelectedIndex = ViewModel.SelectedIndex;
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

        private void BottomNavBar_Unloaded(object sender, EventArgs e)
        {
            BindNavBar(false);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            ViewModel.OnNavigated();
        }
    }
}