using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls
{
    public class LineDataSeries
    {
        public String SeriesLabel { get; set; }
        public List<YEntryData> SeriesData { get; set; }

        public LineDataSeries(List<YEntryData> seriesData)
        {
            if (seriesData == null || seriesData.Count <= 0)
            {
                throw new ArgumentException("Series data cannot be empty or null", nameof(seriesData));
            }
            SeriesData = seriesData;
            SeriesLabel = null;
        }

        public LineDataSeries(String seriesLabel, List<YEntryData> seriesData)
        {
            if (seriesData == null || seriesData.Count <= 0)
            {
                throw new ArgumentException("Series data cannot be empty or null", nameof(seriesData));
            }
            SeriesData = seriesData;
            SeriesLabel = seriesLabel;
        }
    }

    public class XLabelData
    {
        public String XLabel { get; set; }
        public String XIcon { get; set; }
        public int XIconRotation { get; set; }

        public XLabelData(String label)
        {
            XLabel = label;
        }

        public XLabelData(String label, String icon)
            : this(label)
        {
            XIcon = icon;
        }

        public XLabelData(String label, String icon, int iconRotation)
            : this(label, icon)
        {
            XIconRotation = iconRotation;
        }
    }

    public class YEntryData : IComparable<YEntryData>
    {
        public float Y { get; set; }
        public String YLabel { get; set; }

        public YEntryData(float yValue, String label)
        {
            Y = yValue;
            YLabel = label;
        }

        public int CompareTo(YEntryData other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return Y.CompareTo(other.Y);
            }
        }
    }
}
