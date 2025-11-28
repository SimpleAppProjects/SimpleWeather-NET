#nullable enable
using System.Globalization;
using CommunityToolkit.Maui.Converters;
using SimpleWeather.NET.Controls.Graphs;

namespace SimpleWeather.Maui.Converters;

public class GraphDataLabelConverter : BaseConverterOneWay<object?, string?>
{
    public override string? DefaultConvertReturnValue { get; set; } = null;

    public override string? ConvertFrom(object? value, CultureInfo? culture)
    {
        if (value is BarGraphData barGraphData)
        {
            if (!string.IsNullOrWhiteSpace(barGraphData.GraphLabel) && !string.IsNullOrWhiteSpace(barGraphData.GetDataSet()?.Label))
            {
                return $"{barGraphData.GraphLabel} ({barGraphData.GetDataSet().Label})";
            }
            else
            {
                return barGraphData.GraphLabel;
            }
        }
        else if (value is ForecastRangeBarGraphData rangeGraphData)
        {
            if (!string.IsNullOrWhiteSpace(rangeGraphData.GraphLabel) && !string.IsNullOrWhiteSpace(rangeGraphData.GetDataSet()?.Label))
            {
                return $"{rangeGraphData.GraphLabel} ({rangeGraphData.GetDataSet().Label})";
            }
            else
            {
                return rangeGraphData.GraphLabel;
            }
        }

        return value?.ToString();
    }
}