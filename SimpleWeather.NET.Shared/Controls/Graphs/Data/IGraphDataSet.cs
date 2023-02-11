using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.NET.Controls.Graphs
{
    interface IGraphDataSet : IGraphDataBase, IEnumerable
    {
        void AddEntry(object? entry);
        bool RemoveEntry(object? entry);
        int GetEntryIndex(object? entry);
        object? GetEntryForIndex(int index);
        void NotifyDataSetChanged();
    }

    interface IGraphDataSet<T> : IGraphDataSet, IEnumerable<T> where T : GraphEntry
    {
        void AddEntry(T entry);
        bool RemoveEntry(T entry);
        int GetEntryIndex(T entry);
        T GetEntryForIndex(int index);
    }
}
