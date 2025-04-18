﻿using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace SimpleWeather.Maui.Converters
{
    [AcceptEmptyServiceProvider]
    public class DoubleToBooleanConverter : BaseConverterOneWay<object?, bool>
    {
        public double GreaterThan
        {
            get => (double)GetValue(GreaterThanProperty);
            set => SetValue(GreaterThanProperty, value);
        }

        public static readonly BindableProperty GreaterThanProperty =
            BindableProperty.Create(nameof(GreaterThan), typeof(double), typeof(DoubleToBooleanConverter), double.NaN);

        public double LessThan
        {
            get => (double)GetValue(LessThanProperty);
            set => SetValue(LessThanProperty, value);
        }

        public static readonly BindableProperty LessThanProperty =
            BindableProperty.Create(nameof(LessThan), typeof(double), typeof(DoubleToBooleanConverter), double.NaN);

        public bool IsInverse
        {
            get => (bool)GetValue(IsInverseProperty);
            set => SetValue(IsInverseProperty, value);
        }

        public static readonly BindableProperty IsInverseProperty =
            BindableProperty.Create(nameof(IsInverse), typeof(bool), typeof(DoubleToBooleanConverter), false);

        public override bool DefaultConvertReturnValue { get; set; } = false;

        public override bool ConvertFrom(object? value, CultureInfo? culture)
        {
            if (value == null)
            {
                return DefaultConvertReturnValue;
            }

            double vd = 0.0; // DEFAULT?
            if (value is double dbl)
            {
                vd = dbl;
            }
            else if (double.TryParse(value.ToString(), out double result))
            {
                vd = result;
            }

            var boolValue = false;

            if (!double.IsNaN(GreaterThan) && !double.IsNaN(LessThan) &&
                vd > GreaterThan && vd < LessThan)
            {
                boolValue = true;
            }
            else if (!double.IsNaN(GreaterThan) && vd > GreaterThan)
            {
                boolValue = true;
            }
            else if (!double.IsNaN(LessThan) && vd < LessThan)
            {
                boolValue = true;
            }

            // Negate if needed
            if (IsInverse)
            {
                boolValue = !boolValue;
            }

            return boolValue;
        }
    }
}
