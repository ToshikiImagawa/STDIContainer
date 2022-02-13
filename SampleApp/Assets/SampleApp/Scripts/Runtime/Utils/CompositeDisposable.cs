using System;
using System.Collections.Generic;

namespace SampleApp.Utils
{
    public class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private bool _disposed;
        private readonly object _lockObj = new object();

        public CompositeDisposable Add(IDisposable disposable)
        {
            lock (_lockObj)
            {
                if (_disposed)
                {
                    disposable.Dispose();
                }
                else
                {
                    _disposables.Add(disposable);
                }
            }

            return this;
        }

        public void Dispose()
        {
            IDisposable[] disposables;
            lock (_lockObj)
            {
                if (_disposed) return;
                _disposed = true;
                disposables = _disposables.ToArray();
                _disposables.Clear();
            }

            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}