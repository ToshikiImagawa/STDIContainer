using System;

namespace SampleApp.Utils
{
    public class Subject<T> : IObserver<T>, IObservable<T>, IDisposable
    {
        private IObserver<T> _observer = EmptyObserver<T>.Instance;
        private bool _isDisposed;
        private readonly object _gate = new object();

        public void OnCompleted()
        {
            if (_isDisposed) throw new ObjectDisposedException($"call {nameof(OnCompleted)}");
            _observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            if (_isDisposed) throw new ObjectDisposedException($"call {nameof(OnError)}");
            _observer.OnError(error);
        }

        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_gate)
            {
                if (_isDisposed) throw new ObjectDisposedException($"call {nameof(Subscribe)}");
                switch (_observer)
                {
                    case EmptyObserver<T> _:
                    {
                        _observer = observer;
                        return new Subscriber(this, observer);
                    }
                    case ListObserver<T> listObserver:
                    {
                        _observer = listObserver.Add(observer);
                        return new Subscriber(this, observer);
                    }
                    default:
                    {
                        _observer = new ListObserver<T>(new ImmutableList<T>(new[]
                        {
                            _observer,
                            observer
                        }));
                        return new Subscriber(this, observer);
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _isDisposed = true;
                _observer = DisposedObserver<T>.Instance;
            }
        }

        private class Subscriber : IDisposable
        {
            private Subject<T> _root;
            private IObserver<T> _target;
            private readonly object _gate = new object();

            public Subscriber(Subject<T> root, IObserver<T> target)
            {
                _root = root;
                _target = target;
            }

            public void Dispose()
            {
                lock (_gate)
                {
                    if (_root == null) return;
                    lock (_root._gate)
                    {
                        _root._observer = _root._observer is ListObserver<T> listObserver
                            ? listObserver.Remove(_target)
                            : EmptyObserver<T>.Instance;

                        _root = null;
                        _target = null;
                    }
                }
            }
        }
    }
}