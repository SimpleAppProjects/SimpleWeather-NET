using SimpleWeather.SkiaSharp;
using System;
using System.Collections.Generic;

namespace SimpleWeather.NET.Controls.Graphs
{
    public abstract class GraphEntry
    {
        public string XLabel { get; set; }
        public SKDrawable XIcon { get; set; }
        public int XIconRotation { get; set; }

        public override bool Equals(object obj)
        {
            return obj is GraphEntry entry &&
                   XLabel == entry.XLabel &&
                   EqualityComparer<SKDrawable>.Default.Equals(XIcon, entry.XIcon) &&
                   XIconRotation == entry.XIconRotation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(XLabel, XIcon, XIconRotation);
        }
    }
}
