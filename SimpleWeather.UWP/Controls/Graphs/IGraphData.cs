using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.UWP.Controls.Graphs
{
    internal interface IGraphDataBase
    {
        bool IsEmpty { get; }
        int DataCount { get; }

        void Clear();
    }

    interface IGraphData : IGraphDataBase, IEnumerable
    {
        object? GetDataSetByIndex(int index);
        void AddDataSet(object? set);
        bool RemoveDataSet(object? set);
        int GetMaxDataSetLabelCount();
        void NotifyDataChanged();
    }

    interface IGraphData<T, E> : IGraphData, IEnumerable<T> where T : IGraphDataSet<E> where E : GraphEntry
    {
        T GetDataSetByIndex(int index);
        void AddDataSet(T set);
        bool RemoveDataSet(T set);
        int GetMaxDataSetLabelCount();
    }
}
