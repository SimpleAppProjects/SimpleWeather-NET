using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public class BarGraphEntry : GraphEntry
    {
        public YEntryData EntryData { get; set; }

        public Color? FillColor { get; set; }

        public BarGraphEntry() { }

        public BarGraphEntry(string label, YEntryData entryData, string icon = null, int iconRotation = 0)
        {
            this.XLabel = label;
            this.EntryData = entryData;
            this.XIcon = icon;
            this.XIconRotation = iconRotation;
        }
    }
}
