using System;
using System.Linq;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Controls.Shapes;
using MauiControls = Microsoft.Maui.Controls;

namespace SimpleWeather.Maui.Preferences
{
    public partial class PreferenceListDialogPage : ContentPage
    {
        private readonly ListViewCell ListPreference;

        private SettingsChanged ChangedEvent = null;

        protected MauiControls.ListView _PreferenceListView => this.PreferenceListView;

        public PreferenceListDialogPage(ListViewCell listPreference)
        {
            this.BindingContext = this.ListPreference = listPreference;
            this.InitializeComponent();

            PreferenceListItemViewModel selectedItem = null;
            this.PreferenceListView.ItemsSource = ListPreference.Items.Cast<PreferenceListItem>().Select(it =>
            {
                var item = new PreferenceListItemViewModel(it) { IsChecked = Equals(it.Value, listPreference.SelectedItem) };
                if (item.IsChecked)
                {
                    selectedItem = item;
                }
                return item;
            });
            this.PreferenceListView.SelectedItem = selectedItem;
            this.PreferenceListView.ItemSelected += PreferenceListView_ItemSelected;

            this.PreferenceListView.Loaded += (s, e) =>
            {
                if (PreferenceListView.ItemsSource?.OfType<object>()?.Count() is int count)
                {
                    (PreferenceListView.Parent as View).HeightRequest = ((PreferenceListView.RowHeight > 0 ? PreferenceListView.RowHeight : 48) * count) + (count * 0.5);
                }
                else
                {
                    (PreferenceListView.Parent as View).HeightRequest = -1;
                }
            };
        }

        private async void PreferenceListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is PreferenceListItemViewModel item)
            {
                ChangedEvent = new SettingsChanged(ListPreference.PreferenceKey, item.Value);
                WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(ChangedEvent));
                await this.Navigation.PopAsync();
            }
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }
    }
}

