using Microsoft.UI.Xaml.Data;
using SimpleWeather.NET.Controls.Graphs;

namespace SimpleWeather.NET.Converters
{
    public partial class GraphDataLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
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

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
