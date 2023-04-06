using System;
using Microsoft.Maui.Layouts;

namespace SimpleWeather.Maui.Controls
{
	public class AspectRatioFrame : Frame
	{
        public float AspectRatio
        {
            get => (float)GetValue(AspectRatioProperty);
            set => SetValue(AspectRatioProperty, value);
        }

        public static readonly BindableProperty AspectRatioProperty =
            BindableProperty.Create(nameof(AspectRatio), typeof(float), typeof(AspectRatioFrame), 1f, propertyChanged: OnAspectRatioChanged);

        public bool EnableAspectRatio
        {
            get => (bool)GetValue(EnableAspectRatioProperty);
            set => SetValue(EnableAspectRatioProperty, value);
        }

        public static readonly BindableProperty EnableAspectRatioProperty =
            BindableProperty.Create(nameof(EnableAspectRatio), typeof(bool), typeof(AspectRatioFrame), false, propertyChanged: OnEnableAspectRatioChanged);

        private static void OnAspectRatioChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                (bindable as IView)?.InvalidateMeasure();
            }
        }

        private static void OnEnableAspectRatioChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                (bindable as IView)?.InvalidateMeasure();
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var sizeRequest = base.OnMeasure(widthConstraint, heightConstraint);

            return sizeRequest;
        }

        protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
        {
            var width = widthConstraint;
            var height = heightConstraint;

            if (EnableAspectRatio && AspectRatio > 0)
            {
                if (double.IsNormal(widthConstraint))
                {
                    width = ResolveMeasuredWidth(widthConstraint);
                    height = width / AspectRatio;
                }
                else if (double.IsNormal(heightConstraint))
                {
                    height = ResolveMeasuredHeight(heightConstraint);
                    width = height / AspectRatio;
                }
            }
            else
            {
                height = ResolveMeasuredHeight(heightConstraint);
                width = ResolveMeasuredWidth(widthConstraint);
            }

            return base.MeasureOverride(width, height);
        }

        private double ResolveMeasuredHeight(double heightConstraint)
        {
            if (double.IsNormal(heightConstraint) && heightConstraint > 0)
            {
                return ResolveMeasure(MinimumHeightRequest, MaximumHeightRequest, heightConstraint);
            }
            else
            {
                return heightConstraint;
            }
        }

        private double ResolveMeasuredWidth(double widthConstraint)
        {
            if (double.IsNormal(widthConstraint) && widthConstraint > 0)
            {
                return ResolveMeasure(MinimumWidthRequest, MaximumWidthRequest, widthConstraint);
            }
            else
            {
                return widthConstraint;
            }
        }

        private double ResolveMeasure(double min, double max, double preferred)
        {
            if (preferred >= 0 || min == max)
            {
                return preferred < min ? min : (preferred > max ? max : preferred);
            }
            else if (double.IsNormal(max))
            {
                return Math.Min(max, preferred);
            }
            else
            {
                return preferred;
            }
        }
    }
}

