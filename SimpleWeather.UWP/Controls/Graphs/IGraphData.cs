using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls.Graphs
{
    public interface IGraphData
    {
        bool IsEmpty { get; }
        int DataCount { get; }
    }
}
