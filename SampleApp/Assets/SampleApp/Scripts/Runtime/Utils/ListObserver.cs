using System;
using SampleApp.Model;

namespace SampleApp.Utils
{
    public class ListObserver<T> : IObserver<T>
    {
        private readonly ImmutableList<T> _observers;

        public ListObserver(ImmutableList<T> observers)
        {
            _observers = observers;
        }

        public void OnCompleted()
        {
            var targetObservers = _observers.Data;
            foreach (var observer in targetObservers)
            {
                observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            var targetObservers = _observers.Data;
            foreach (var observer in targetObservers)
            {
                observer.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            var targetObservers = _observers.Data;
            foreach (var observer in targetObservers)
            {
                observer.OnNext(value);
            }
        }

        internal IObserver<T> Add(IObserver<T> observer)
        {
            return new ListObserver<T>(_observers.Add(observer));
        }

        internal IObserver<T> Remove(IObserver<T> observer)
        {
            var i = Array.IndexOf(_observers.Data, observer);
            if (i < 0) return this;
            return _observers.Data.Length == 2
                ? _observers.Data[1 - i]
                : new ListObserver<T>(_observers.Remove(observer));
        }
    }
}