using System.Collections;

namespace SimpleWeather.NET.Controls.Graphs
{
    public interface IGraphDataBase
    {
        bool IsEmpty { get; }
        int DataCount { get; }

        void Clear();
    }

    public interface IGraphData : IGraphDataBase, IEnumerable
    {
        object? GetDataSetByIndex(int index);
        void AddDataSet(object? set);
        bool RemoveDataSet(object? set);
        int GetMaxDataSetLabelCount();
        void NotifyDataChanged();
    }

    public interface IGraphData<T, E> : IGraphData, IEnumerable<T> where T : IGraphDataSet<E> where E : GraphEntry
    {
        T GetDataSetByIndex(int index);
        void AddDataSet(T set);
        bool RemoveDataSet(T set);
        int GetMaxDataSetLabelCount();
    }
}
