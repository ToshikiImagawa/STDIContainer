// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;

namespace SampleApp.Utils
{
    public class DisposedObserver<T> : IObserver<T>
    {
        public static readonly DisposedObserver<T> Instance = new DisposedObserver<T>();

        public void OnCompleted()
        {
            throw new ObjectDisposedException($"call {nameof(OnCompleted)}: {typeof(T).Name}");
        }

        public void OnError(Exception error)
        {
            throw new ObjectDisposedException($"call {nameof(OnCompleted)}: {typeof(T).Name}");
        }

        public void OnNext(T value)
        {
            throw new ObjectDisposedException($"call {nameof(OnCompleted)}: {typeof(T).Name}");
        }
    }
}