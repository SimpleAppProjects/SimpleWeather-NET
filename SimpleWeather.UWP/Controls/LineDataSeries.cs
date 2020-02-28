using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpleWeather.UWP.Controls
{
    public class LineDataSeries
    {
        public String SeriesLabel { get; set; }
        public List<YEntryData> SeriesData { get; private set; }

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

        public override bool Equals(object obj)
        {
            return obj is LineDataSeries series &&
                   SeriesLabel == series.SeriesLabel &&
                   EqualityComparer<List<YEntryData>>.Default.Equals(SeriesData, series.SeriesData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SeriesLabel, SeriesData);
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

        public override bool Equals(object obj)
        {
            return obj is XLabelData data &&
                   XLabel == data.XLabel &&
                   XIcon == data.XIcon &&
                   XIconRotation == data.XIconRotation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(XLabel, XIcon, XIconRotation);
        }
    }

    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "<Pending>")]
    public class YEntryData : IComparable<YEntryData>, IEquatable<YEntryData>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as YEntryData);
        }

        public bool Equals(YEntryData other)
        {
            return other != null &&
                   Y == other.Y &&
                   YLabel == other.YLabel;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Y, YLabel);
        }
    }
}