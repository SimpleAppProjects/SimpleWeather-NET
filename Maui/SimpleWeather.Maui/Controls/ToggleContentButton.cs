using System;
using SimpleToolkit.Core;

namespace SimpleWeather.Maui.Controls
{
	public class ToggleContentButton : ContentButton
	{
		public bool IsChecked
		{
			get => (bool)GetValue(IsCheckedProperty);
			set => SetValue(IsCheckedProperty, value);
		}

		public static readonly BindableProperty IsCheckedProperty =
			BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(ToggleContentButton), false);

        public override void OnClicked()
        {
            base.OnClicked();
			IsChecked = !IsChecked;
        }
    }
}

