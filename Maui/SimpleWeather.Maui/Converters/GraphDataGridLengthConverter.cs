#nullable enable
using System;
using System.Globalization;
using CommunityToolkit.Maui.Converters;
using SimpleWeather.NET.Controls.Graphs;

namespace SimpleWeather.Maui.Converters
{
    public class GraphDataGridLengthConverter : BaseConverterOneWay<object?, GridLength>
    {
        public override GridLength DefaultConvertReturnValue { get; set; } = default;

        public override GridLength ConvertFrom(object? value, CultureInfo? culture)
        {
            if (value is IGraphData graphData && graphData.GetDataSetByIndex(0) is IGraphDataSet)
            {
                return GridLength.Auto;
            }

            return new GridLength(0);
        }
    }
}
#nullable disable