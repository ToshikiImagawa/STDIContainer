// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp.Utils
{
    public static class SynchronizationContextExtensions
    {
        public static async Task<T> Post<T>(this SynchronizationContext self, Func<Task<T>> callback)
        {
            return await self.Post(_ => { return callback(); }, null);
        }

        public static async Task<T> Post<T>(this SynchronizationContext self, Func<T> callback)
        {
            return await self.Post(_ => { return callback(); }, null);
        }

        private static async Task<T> Post<T>(
            this SynchronizationContext self, Func<object, Task<T>> callback,
            object state
        )
        {
            var posted = false;
            T value = default;

            async void SendOrPostCallback(object callbackState)
            {
                value = await callback(callbackState);
                posted = true;
            }

            self.Post(SendOrPostCallback, state);
            await Task.Run(async () =>
            {
                while (!posted) await Task.Delay(1);
            });
            return value;
        }

        private static async Task<T> Post<T>(
            this SynchronizationContext self, Func<object, T> callback,
            object state
        )
        {
            var posted = false;
            T value = default;

            void SendOrPostCallback(object callbackState)
            {
                value = callback(callbackState);
                posted = true;
            }

            self.Post(SendOrPostCallback, state);
            await Task.Run(async () =>
            {
                while (!posted) await Task.Delay(1);
            });
            return value;
        }
    }
}