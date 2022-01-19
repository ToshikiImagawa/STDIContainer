// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STDIC.Internal
{
    internal sealed class ManagedCollection<T> : ICollection<T>, IDisposable where T : class
    {
        private const int SHRINK_THRESHOLD = 64;
        private List<T> _items;
        private readonly object _lockObj = new object();

        public ManagedCollection()
        {
            _items = new List<T>();
        }

        public ManagedCollection(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException(nameof(capacity));
            _items = new List<T>(capacity);
        }

        public void Add(T item)
        {
            Add(item, out _);
        }

        public void Add(T item, out bool disposed)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            lock (_lockObj)
            {
                disposed = IsDisposed;
                if (!disposed)
                {
                    _items.Add(item);
                    Count++;
                }
            }
        }

        public void Clear()
        {
            Clear(out _);
        }

        public void Clear(out T[] oldItems)
        {
            lock (_lockObj)
            {
                oldItems = _items.ToArray();
                _items.Clear();
                Count = 0;
            }
        }

        public bool Contains(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            lock (_lockObj)
            {
                return _items.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            lock (_lockObj)
            {
                Array.Copy(
                    _items.Where(item => item != null).ToArray(),
                    0,
                    array,
                    arrayIndex,
                    array.Length - arrayIndex
                );
            }
        }

        public bool Remove(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            lock (_lockObj)
            {
                if (IsDisposed) return false;
                var index = _items.IndexOf(item);
                if (index < 0) return false;
                _items[index] = null;
                Count--;

                if (_items.Capacity <= SHRINK_THRESHOLD || Count >= _items.Capacity / 2) return true;
                var old = _items;
                _items = new List<T>(_items.Capacity / 2);
                _items.AddRange(old.Where(disposable => disposable != null));
                return true;
            }
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> currentItems;
            lock (_lockObj)
            {
                currentItems = _items.Where(disposable => disposable != null).ToArray();
            }

            return currentItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            lock (_lockObj)
            {
                if (IsDisposed) return;
                IsDisposed = true;
                _items.Clear();
                Count = 0;
            }
        }

        public bool IsDisposed { get; private set; }
    }
}