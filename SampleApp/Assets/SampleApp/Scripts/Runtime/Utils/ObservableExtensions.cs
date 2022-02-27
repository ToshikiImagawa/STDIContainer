using System;
using SampleApp.Model;

namespace SampleApp.Utils
{
    public static class ObservableExtensions
    {
        public static IDisposable Subscribe<T>(this IObservable<T> self, Action<T> onNext)
        {
            return self.Subscribe(new ActionObserver<T>(onNext));
        }

        private class ActionObserver<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;
            private readonly Action _onCompleted;
            private readonly Action<Exception> _onError;

            public ActionObserver(
                Action<T> onNext = null,
                Action onCompleted = null,
                Action<Exception> onError = null
            )
            {
                _onNext = onNext;
                _onCompleted = onCompleted;
                _onError = onError;
            }

            public void OnCompleted()
            {
                _onCompleted?.Invoke();
            }

            public void OnError(Exception error)
            {
                _onError?.Invoke(error);
            }

            public void OnNext(T value)
            {
                _onNext?.Invoke(value);
            }
        }
    }
}