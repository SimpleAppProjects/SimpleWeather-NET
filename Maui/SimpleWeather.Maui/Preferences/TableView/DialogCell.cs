using System;
using CommunityToolkit.Maui.Markup;

namespace SimpleWeather.Maui.Preferences
{
	public abstract class DialogCell : TextCell
	{
		public string PreferenceKey
		{
			get => (string)GetValue(PreferenceKeyProperty);
			set => SetValue(PreferenceKeyProperty, value);
		}

		public static readonly BindableProperty PreferenceKeyProperty =
			BindableProperty.Create(nameof(PreferenceKey), typeof(string), typeof(DialogCell), null);
	}
}

