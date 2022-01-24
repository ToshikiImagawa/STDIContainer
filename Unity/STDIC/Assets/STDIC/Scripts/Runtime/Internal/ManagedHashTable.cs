// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace STDIC.Internal
{
    internal class ManagedHashTable<TKey, TValue>
    {
        private Entry[] _entries;
        private int _size;
        private readonly object _writerLock = new object();
        private readonly EqualityComparer<TKey> _equalityComparer;

        public ManagedHashTable() : this(EqualityComparer<TKey>.Default)
        {
        }

        public ManagedHashTable(IEnumerable<(TKey, TValue)> values) : this(values, EqualityComparer<TKey>.Default)
        {
        }

        public ManagedHashTable(EqualityComparer<TKey> equalityComparer)
        {
            _entries = new Entry[16];
            _equalityComparer = equalityComparer;
        }

        public ManagedHashTable(IEnumerable<(TKey, TValue)> values, EqualityComparer<TKey> equalityComparer)
        {
            _entries = new Entry[16];
            foreach (var (key, value) in values)
            {
                TryAdd(key, value);
            }

            _equalityComparer = equalityComparer;
        }

        public TValue[] Values => ValuesInternal.ToArray();

        private IEnumerable<TValue> ValuesInternal
        {
            get
            {
                var entries = _entries;
                foreach (var e in entries)
                {
                    var entry = e;
                    if (entry == null) continue;
                    yield return entry.Value;
                    while (entry.Next != null)
                    {
                        entry = entry.Next;
                        yield return entry.Value;
                    }
                }
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return TryAdd(key, _ => value);
        }

        public bool TryAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            bool success;
            lock (_writerLock)
            {
                success = TryAddInternal(key, valueFactory, out _);
            }

            return success;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var entries = _entries;
            var hash = key.GetHashCode();
            var entry = entries[hash & entries.Length - 1];
            while (entry != null)
            {
                if (CheckKeyEquals(entry.Key, key))
                {
                    value = entry.Value;
                    return true;
                }

                entry = entry.Next;
            }

            value = default;
            return false;
        }

        public TValue GetOrAddValue(TKey key, TValue value)
        {
            return GetOrAddValue(key, _ => value);
        }

        public TValue GetOrAddValue(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value;
            lock (_writerLock)
            {
                if (!TryGetValue(key, out value))
                {
                    if (!TryAddInternal(key, valueFactory, out value))
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            return value;
        }

        private bool TryAddInternal(TKey key, Func<TKey, TValue> valueFactory, out TValue resultingValue)
        {
            var nextCapacity = CalculateCapacity(_size + 1);
            if (_entries.Length < nextCapacity)
            {
                var nextEntries = new Entry[nextCapacity];
                foreach (var entry in _entries)
                {
                    var nextEntry = entry;
                    while (nextEntry != null)
                    {
                        var newEntry = new Entry(nextEntry.Key, nextEntry.Value, nextEntry.Hash);
                        AddToEntries(nextEntries, key, newEntry, null, out resultingValue);
                        nextEntry = nextEntry.Next;
                    }
                }

                var successAdd = AddToEntries(nextEntries, key, null, valueFactory, out resultingValue);
                VolatileWrite(ref _entries, nextEntries);
                if (successAdd)
                {
                    _size++;
                }

                return successAdd;
            }
            else
            {
                var successAdd = AddToEntries(_entries, key, null, valueFactory, out resultingValue);
                if (successAdd)
                {
                    _size++;
                }

                return successAdd;
            }
        }

        private bool AddToEntries(
            Entry[] entries,
            TKey newKey,
            Entry newEntryOrNull,
            Func<TKey, TValue> valueFactory,
            out TValue resultingValue)
        {
            var hashCode = newEntryOrNull?.Hash ?? newKey.GetHashCode();
            if (entries[hashCode & (entries.Length - 1)] == null)
            {
                resultingValue = newEntryOrNull != null ? newEntryOrNull.Value : valueFactory(newKey);
                VolatileWrite(
                    ref entries[hashCode & (entries.Length - 1)],
                    newEntryOrNull ?? new Entry(newKey, resultingValue, hashCode)
                );
            }
            else
            {
                var searchLastEntry = entries[hashCode & (entries.Length - 1)];
                while (true)
                {
                    if (CheckKeyEquals(searchLastEntry.Key, newKey))
                    {
                        resultingValue = searchLastEntry.Value;
                        return false;
                    }

                    if (searchLastEntry.Next == null)
                    {
                        resultingValue = newEntryOrNull != null ? newEntryOrNull.Value : valueFactory(newKey);
                        VolatileWrite(
                            ref searchLastEntry.Next,
                            newEntryOrNull ?? new Entry(newKey, resultingValue, hashCode)
                        );
                        break;
                    }

                    searchLastEntry = searchLastEntry.Next;
                }
            }

            return true;
        }

        protected virtual bool CheckKeyEquals(TKey left, TKey right)
        {
            return _equalityComparer.Equals(left, right);
        }

        private static void VolatileWrite(ref Entry[] location, Entry[] value)
        {
            System.Threading.Volatile.Write(ref location, value);
        }

        private static void VolatileWrite(ref Entry location, Entry value)
        {
            System.Threading.Volatile.Write(ref location, value);
        }

        private static int CalculateCapacity(int size)
        {
            var initialCapacity = (int)(size / 0.75f);
            var capacity = 1;
            while (capacity < initialCapacity)
            {
                capacity <<= 1;
            }

            return capacity < 8 ? 8 : capacity;
        }

        private class Entry
        {
            public Entry(TKey key, TValue value, int hash)
            {
                Key = key;
                Value = value;
                Hash = hash;
                Next = null;
            }

            internal TKey Key { get; }
            internal TValue Value { get; }
            internal int Hash { get; }
            internal Entry Next;
        }
    }
}