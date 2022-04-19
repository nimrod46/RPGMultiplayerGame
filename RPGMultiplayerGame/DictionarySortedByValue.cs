using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RPGMultiplayerGame
{
    public class DictionarySortedByValue<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private class ValueWrapper : IComparable, IComparable<ValueWrapper>
        {
            public TKey Key { get; }
            public TValue Value { get; }

            public ValueWrapper(TKey k, TValue v)
            {
                Key = k;
                Value = v;
            }
            public int CompareTo(object obj)
            {
                if (obj is not ValueWrapper wrapper)
                    throw new ArgumentException("obj is not a ValueWrapper type object");
                return CompareTo(wrapper);
            }
            public int CompareTo(ValueWrapper other)
            {
                int c = Comparer<TValue>.Default.Compare(Value, other.Value);
                return c;
            }
        }

        private readonly List<ValueWrapper> orderedElements;
        private readonly Dictionary<TKey, TValue> innerDict;

        public DictionarySortedByValue()
        {
            orderedElements = new List<ValueWrapper>();
            innerDict = new Dictionary<TKey, TValue>();
        }

        public void Add(TKey key, TValue value)
        {
            innerDict.Add(key, value);
            var wrap = new ValueWrapper(key, value);
            if (orderedElements.Count == 0)
            {
                orderedElements.Add(wrap);
                return;
            }
            if (orderedElements[0].CompareTo(wrap) >= 0)
            {
                orderedElements.Insert(0, wrap);
                return;
            }
            if (orderedElements[orderedElements.Count - 1].CompareTo(wrap) <= 0)
            {
                orderedElements.Add(wrap);
                return;
            }
            int index = orderedElements.BinarySearch(wrap);
            if (index < 0)
                index = ~index;
            orderedElements.Insert(index, wrap);
        }

        public KeyValuePair<TKey, TValue>? GetMaxElement()
        {
            if (orderedElements.Count == 0)
            {
                return null;
            }
            return new KeyValuePair<TKey, TValue>(orderedElements[orderedElements.Count - 1].Key, orderedElements[orderedElements.Count - 1].Value);
        }

        public bool ContainsKey(TKey key)
        {
            return orderedElements.Any(i => i.Key!.Equals(key));
        }

        public ICollection<TKey> Keys
        {
            get { return orderedElements.Select(i => i.Key).ToList(); }
        }

        public bool Remove(TKey key)
        {
            if (TryGetValue(key, out _))
            {
                orderedElements.RemoveAll(i => i.Key!.Equals(key));
                innerDict.Remove(key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return innerDict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return innerDict.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return innerDict[key];
            }
            set
            {
                _ = Remove(key);
                Add(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            innerDict.Clear();
            orderedElements.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return innerDict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            innerDict.ToList().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return innerDict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Contains(item))
                return Remove(item.Key);
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var el in orderedElements)
                yield return new KeyValuePair<TKey, TValue>(el.Key, el.Value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
