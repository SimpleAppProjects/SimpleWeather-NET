using System;
using System.Globalization;
using CommunityToolkit.Maui.Converters;
using SimpleWeather.NET.Controls.Graphs;

namespace SimpleWeather.Maui.Converters
{
    public class GraphDataVisibilityConverter : BaseConverterOneWay<object, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; } = false;

        public override bool ConvertFrom(object value, CultureInfo culture)
        {
            if (value is IGraphData graphData && graphData.GetDataSetByIndex(0) is IGraphDataSet graphDataSet)
            {
                return graphDataSet.DataCount > 0;
            }

            return false;
        }
    }
}

