using System;

namespace SimpleWeather.NET.Controls.Graphs
{
    public class YEntryData : IComparable<YEntryData>, IEquatable<YEntryData>
    {
        public float Y { get; set; }
        public String YLabel { get; set; }

        public YEntryData(float yValue, String label)
        {
            Y = yValue;
            YLabel = label ?? throw new ArgumentNullException(nameof(label));
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