using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleWeather.Maui.Preferences
{
	public partial class PreferenceListItemViewModel : ObservableObject
	{
		[ObservableProperty]
		private string display;
        [ObservableProperty]
        private object value;

        [ObservableProperty]
        private string detail;

        [ObservableProperty]
        private bool isChecked;

		public PreferenceListItemViewModel()
		{
		}

        public PreferenceListItemViewModel(PreferenceListItem item)
        {
            this.Display = item.Display;
            this.Value = item.Value;
            this.Detail = item.Detail;
        }
    }
}

