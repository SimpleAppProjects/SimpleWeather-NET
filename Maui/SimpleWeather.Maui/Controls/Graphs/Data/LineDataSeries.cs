using System.Collections.Generic;
using System.Linq;

namespace SimpleWeather.Maui.Controls.Graphs
{
    public class LineDataSeries : GraphDataSet<LineGraphEntry>
    {
        private readonly Color[] DefaultColors = { Color.FromRgba(0, 0x70, 0xC0, 0xFF), Colors.LightSeaGreen, Colors.YellowGreen };

        public string SeriesLabel { get; set; }
        public List<Color> SeriesColors { get; private set; }

        public float? SeriesMin { get; set; }
        public float? SeriesMax { get; set; }

        public LineDataSeries(IEnumerable<LineGraphEntry> seriesData) : base(seriesData)
        {
            SeriesLabel = null;
            SeriesColors = DefaultColors.ToList();
        }

        public LineDataSeries(string seriesLabel, List<LineGraphEntry> seriesData) : this(seriesData)
        {
            SeriesLabel = seriesLabel;
        }

        public Color GetColor(int idx)
        {
            return SeriesColors[idx % SeriesColors.Count];
        }

        public void SetSeriesColors(params Color[] colors)
        {
            SeriesColors = new List<Color>(colors);
        }

        public void SetSeriesMinMax(float? seriesMin, float? seriesMax)
        {
            SeriesMin = seriesMin;
            SeriesMax = seriesMax;
        }

        protected override void CalcMinMax(LineGraphEntry entry)
        {
            if (SeriesMin == null)
            {
                if (entry.YEntryData.Y < YMin)
                {
                    YMin = entry.YEntryData.Y;
                }
            }
            if (SeriesMax == null)
            {
                if (entry.YEntryData.Y > YMax)
                {
                    YMax = entry.YEntryData.Y;
                }
            }
        }
    }
}