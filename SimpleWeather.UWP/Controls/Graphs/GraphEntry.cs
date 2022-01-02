using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public abstract class GraphEntry
    {
        public String XLabel { get; set; }
        public String XIcon { get; set; }
        public int XIconRotation { get; set; }

        public override bool Equals(object obj)
        {
            return obj is GraphEntry entry &&
                   XLabel == entry.XLabel &&
                   XIcon == entry.XIcon &&
                   XIconRotation == entry.XIconRotation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(XLabel, XIcon, XIconRotation);
        }
    }
}
