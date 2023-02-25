using System;

namespace SimpleWeather.Maui.Controls
{
    [ContentProperty(nameof(Data))]
    public class HeaderedContentControl : TemplatedContentPresenter
	{
		private const string PartHeaderPresenter = "HeaderPresenter";

        public object Header
		{
			get => (object)GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public static readonly BindableProperty HeaderProperty =
			BindableProperty.Create(nameof(Header), typeof(object), typeof(HeaderedContentControl), null, propertyChanged: OnHeaderChanged);

		public DataTemplate HeaderTemplate
		{
			get => (DataTemplate)GetValue(HeaderTemplateProperty);
			set => SetValue(HeaderTemplateProperty, value);
		}

		public static readonly BindableProperty HeaderTemplateProperty =
			BindableProperty.Create(nameof(HeaderTemplate), typeof(DataTemplate), typeof(HeaderedContentControl), null);

		public StackOrientation Orientation
		{
			get => (StackOrientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public static readonly BindableProperty OrientationProperty =
			BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(HeaderedContentControl), StackOrientation.Vertical);

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

			SetHeaderVisibility();
			SetOrientation();
        }

        /// <summary>
        /// Called when the <see cref="Header"/> property changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="Header"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="Header"/> property.</param>
        protected virtual void OnHeaderChanged(object oldValue, object newValue)
        {
        }

        private void SetHeaderVisibility()
        {
            if (GetTemplateChild(PartHeaderPresenter) is VisualElement headerPresenter)
			{
				if (Header is string headerText)
				{
					headerPresenter.IsVisible = !string.IsNullOrEmpty(headerText);
				}
				else
				{
                    headerPresenter.IsVisible = Header != null;
                }
            }
        }

        private void SetOrientation()
        {
			var orientation = this.Orientation == StackOrientation.Vertical
				? nameof(StackOrientation.Vertical)
				: nameof(StackOrientation.Horizontal);

			VisualStateManager.GoToState(this, orientation);
        }

        private static void OnHeaderChanged(BindableObject bindable, object oldValue, object newValue)
        {
			var control = (HeaderedContentControl)bindable;
			control.SetHeaderVisibility();
			control.OnHeaderChanged(oldValue, newValue);
        }
    }
}

