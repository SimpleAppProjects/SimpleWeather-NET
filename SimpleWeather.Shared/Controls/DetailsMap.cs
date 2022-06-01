using SimpleWeather.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Controls
{
    public sealed partial class DetailsMap<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>, ICollection, IDictionary, IDeserializationCallback, ISerializable, INotifyCollectionChanged where TValue : DetailItemViewModel
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private ObservableCollectionWrapper<TValue> _valueWrapper;
        private readonly Dictionary<TKey, TValue> _dictionary;

        public DetailsMap() : this(0, null) { }

        public DetailsMap(int capacity) : this(capacity, null) { }

        public DetailsMap(IEqualityComparer<TKey>? comparer) : this(0, comparer) { }

        public DetailsMap(int capacity, IEqualityComparer<TKey>? comparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public IReadOnlyCollection<TValue> ValuesWrapper
        {
            get
            {
                if (_valueWrapper == null)
                    _valueWrapper = new ObservableCollectionWrapper<TValue>(this.Values);

                return _valueWrapper;
            }
        }

        public void NotifyCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            _valueWrapper?.NotifyCollectionChanged();
        }
    }
}
