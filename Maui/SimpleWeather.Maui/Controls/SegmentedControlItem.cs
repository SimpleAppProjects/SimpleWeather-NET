using System;
using Plugin.Maui.SegmentedControl;

namespace SimpleWeather.Maui.Controls
{
	public class SegmentedControlItem : SegmentedControlOption
	{
		public object Value
		{
			get => (object)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public static readonly BindableProperty ValueProperty =
			BindableProperty.Create(nameof(Value), typeof(object), typeof(SegmentedControlItem), default);

		public SegmentedControlItem() : base() { }

        public SegmentedControlItem(string title, object value) : base()
		{
			this.Text = title;
			this.Value = value;
		}
    }
}

