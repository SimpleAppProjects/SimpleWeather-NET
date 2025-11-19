using System.Collections;

namespace SimpleWeather.NET.Controls.Graphs
{
    public interface IGraphDataSet : IGraphDataBase, IEnumerable
    {
        void AddEntry(object? entry);
        bool RemoveEntry(object? entry);
        int GetEntryIndex(object? entry);
        object? GetEntryForIndex(int index);
        void NotifyDataSetChanged();
    }

    public interface IGraphDataSet<T> : IGraphDataSet, IEnumerable<T> where T : GraphEntry
    {
        void AddEntry(T entry);
        bool RemoveEntry(T entry);
        int GetEntryIndex(T entry);
        T GetEntryForIndex(int index);
    }
}
