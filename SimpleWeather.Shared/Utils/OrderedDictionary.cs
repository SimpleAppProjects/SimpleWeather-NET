using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.Utils
{
    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly System.Collections.Specialized.OrderedDictionary _dictionary = new System.Collections.Specialized.OrderedDictionary();

        public TValue this[TKey key] { get => (TValue)_dictionary[key]; set => _dictionary[key] = value; }

        public ICollection<TKey> Keys => _dictionary.Keys.Cast<TKey>().ToList();

        public ICollection<TValue> Values => _dictionary.Values.Cast<TValue>().ToList();

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        private IEnumerable<KeyValuePair<TKey, TValue>> KeyValuePairs
        {
            get
            {
                return _dictionary.OfType<DictionaryEntry>().Select(kvp => new KeyValuePair<TKey, TValue>((TKey)kvp.Key, (TValue)kvp.Value));
            }
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.Contains(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            KeyValuePairs.ToList().CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return KeyValuePairs.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            var result = _dictionary.Contains(key);
            if (result)
            {
                _dictionary.Remove(key);
            }
            return result;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (TryGetValue(item.Key, out TValue value)
                && Equals(value, item.Value))
            {
                Remove(item.Key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            object foundValue;
            if ((foundValue = _dictionary[key]) != null
                || _dictionary.Contains(key))
            {
                // Either found with a non-null value, or contained value is null.
                value = (TValue)foundValue;
                return true;
            }
            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
