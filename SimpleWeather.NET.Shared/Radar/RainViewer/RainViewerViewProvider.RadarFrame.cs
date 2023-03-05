using System;

namespace SimpleWeather.NET.Radar.RainViewer
{
    internal sealed class RadarFrame
    {
        public String Host { get; }
        public String Path { get; }
        public long TimeStamp { get; }

        public RadarFrame(long timeStamp, string host, string path)
        {
            Host = host;
            Path = path;
            TimeStamp = timeStamp;
        }
    }
}

