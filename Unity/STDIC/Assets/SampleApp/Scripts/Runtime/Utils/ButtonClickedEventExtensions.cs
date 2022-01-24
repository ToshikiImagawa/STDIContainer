// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SampleApp.Utils
{
    public static class ButtonClickedEventExtensions
    {
        public static IDisposable Subscribe(this Button.ButtonClickedEvent self, UnityAction call)
        {
            self.AddListener(call);
            return new Subscriber(self, call);
        }

        private class Subscriber : IDisposable
        {
            private readonly object _gate = new object();
            private UnityEvent _root;
            private UnityAction _target;

            public Subscriber(UnityEvent root, UnityAction target)
            {
                _root = root;
                _target = target;
            }

            public void Dispose()
            {
                lock (_gate)
                {
                    if (_root == null) return;
                    _root?.RemoveListener(_target);
                    _root = null;
                    _target = null;
                }
            }
        }
    }
}