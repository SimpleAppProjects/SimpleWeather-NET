using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SimpleWeather.Utils
{
    public class SimpleObservableList<T> : List<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SimpleObservableList() : base() { }

        public SimpleObservableList(int capacity) : base(capacity) { }

        public SimpleObservableList(IEnumerable<T> collection) : base(collection) { }

        public void NotifyCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
