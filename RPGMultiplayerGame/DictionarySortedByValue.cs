﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGMultiplayerGame
{
    public class DictionarySortedByValue<TKey, TValue> : IDictionary<TKey, TValue>
    {
        class ValueWrapper : IComparable, IComparable<ValueWrapper>
        {
            public TKey Key { get; private set; }
            public TValue Value { get; private set; }

            public ValueWrapper(TKey k, TValue v)
            {
                this.Key = k;
                this.Value = v;
            }
            public int CompareTo(object obj)
            {
                if (!(obj is ValueWrapper))
                    throw new ArgumentException("obj is not a ValueWrapper type object");
                return this.CompareTo(obj as ValueWrapper);
            }
            public int CompareTo(ValueWrapper other)
            {
                int c = Comparer<TValue>.Default.Compare(this.Value, other.Value);
                return c;
            }
        }

        private readonly SortedSet<ValueWrapper> orderedElements;
        private readonly Dictionary<TKey, TValue> innerDict;
        private ValueWrapper maxValue;

        public DictionarySortedByValue()
        {
            this.orderedElements = new SortedSet<ValueWrapper>();
            this.innerDict = new Dictionary<TKey, TValue>();
            maxValue = null;
        }

        public void Add(TKey key, TValue value)
        {
            var wrap = new ValueWrapper(key, value);
            this.innerDict.Add(key, value);
            this.orderedElements.Add(wrap);
            if (maxValue == null)
            {
                maxValue = wrap;
            }
            else
            {
                maxValue = (wrap.CompareTo(maxValue) == 1 ? wrap : maxValue);
            }
        }

        public KeyValuePair<TKey, TValue>? GetMaxElement()
        {
            if (maxValue == null)
            {
                return null;
            }
            return new KeyValuePair<TKey, TValue>(maxValue.Key, maxValue.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return this.innerDict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return this.innerDict.Keys; }
        }

        public bool Remove(TKey key)
        {
            if (this.TryGetValue(key, out TValue val))
            {
                var wrap = new ValueWrapper(key, val);
                this.orderedElements.Remove(wrap);
                this.innerDict.Remove(key);
                if (maxValue.Key.Equals(key))
                {
                    if (orderedElements.Count != 0)
                    {
                        maxValue = orderedElements.ElementAt(Count - 1);
                    }
                    else
                    {
                        maxValue = null;
                    }
                }
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.innerDict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return this.innerDict.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.innerDict[key];
            }
            set
            {
                _ = this.Remove(key);
                this.Add(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            this.innerDict.Clear();
            this.orderedElements.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var wrap = new ValueWrapper(item.Key, item.Value);
            return this.orderedElements.Contains(wrap);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            innerDict.ToList().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.innerDict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (this.Contains(item))
                return this.Remove(item.Key);
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var el in this.orderedElements)
                yield return new KeyValuePair<TKey, TValue>(el.Key, el.Value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
