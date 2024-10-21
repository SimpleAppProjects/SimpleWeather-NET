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
			BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(ToggleContentButton), false, propertyChanged: OnCheckedPropertyChanged);

		public EventHandler<CheckedChangedEventArgs> CheckedChanged;

        public override void OnClicked()
        {
            base.OnClicked();
			IsChecked = !IsChecked;
        }

		protected virtual void OnCheckedChanged(bool isChecked)
		{
            CheckedChanged?.Invoke(this, new CheckedChangedEventArgs(isChecked));
        }

		private static void OnCheckedPropertyChanged(BindableObject obj, object oldValue, object newValue)
		{
			if (obj is ToggleContentButton btn)
			{
				btn.OnCheckedChanged((bool)newValue);
			}
		}
    }
}

