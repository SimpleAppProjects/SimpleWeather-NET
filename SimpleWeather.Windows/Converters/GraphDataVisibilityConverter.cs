using SimpleWeather.NET.Controls.Graphs;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SimpleWeather.NET.Converters
{
    public class GraphDataVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is IGraphData graphData)
            {
                var graphDataSet = graphData.GetDataSetByIndex(0) as IGraphDataSet;
                return graphDataSet?.DataCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
