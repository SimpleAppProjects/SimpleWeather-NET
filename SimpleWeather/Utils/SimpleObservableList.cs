using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SimpleWeather.Utils
{
    public class SimpleObservableList<T> : List<T>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void NotifyCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
