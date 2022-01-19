// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;

namespace STDIC.Internal
{
    internal sealed class CompositeDisposable : IDisposable, ICollection<IDisposable>
    {
        private readonly ManagedCollection<IDisposable> _disposables;

        public CompositeDisposable()
        {
            _disposables = new ManagedCollection<IDisposable>();
        }

        public CompositeDisposable(int capacity)
        {
            _disposables = new ManagedCollection<IDisposable>(capacity);
        }

        public int Count => _disposables.Count;
        public bool IsReadOnly => _disposables.IsReadOnly;
        public bool IsDisposed => _disposables.IsDisposed;

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        public void Add(IDisposable item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _disposables.Add(item, out var disposed);
            if (disposed) item.Dispose();
        }

        public void Clear()
        {
            _disposables.Clear(out var currentDisposables);
            foreach (var disposable in currentDisposables) disposable?.Dispose();
        }

        public bool Contains(IDisposable item)
        {
            return _disposables.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            _disposables.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDisposable item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var removed = _disposables.Remove(item);
            if (removed) item.Dispose();
            return removed;
        }
    }
}