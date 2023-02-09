using System;
using System.Collections;
using SimpleWeather.Controls;

namespace SimpleWeather.Maui.Preferences
{
	public class ListViewCell : DialogCell
	{
		public IEnumerable Items
		{
			get => (IEnumerable)GetValue(ItemsProperty);
			set => SetValue(ItemsProperty, value);
		}

		public static readonly BindableProperty ItemsProperty =
			BindableProperty.Create(nameof(Items), typeof(IEnumerable), typeof(ListViewCell), new List<PreferenceListItem>(0));

		public object SelectedItem
		{
			get => (object)GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ListViewCell), null);
	}
}

