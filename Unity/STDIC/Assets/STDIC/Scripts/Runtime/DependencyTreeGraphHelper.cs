// Copyright (c) 2022 COMCREATE. All rights reserved.

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using STDIC.Internal;
using UnityEngine;

namespace STDIC
{
    internal class DependencyTreeGraphHelper : IObservable<DependencyTreeGraphHelper.ContainerData>
    {
        private readonly List<ContainerData> _cache = new List<ContainerData>();
        [NotNull] public static DependencyTreeGraphHelper Instance { get; }

        [NotNull] public ContainerData[] Values => _cache.ToArray();

        static DependencyTreeGraphHelper()
        {
            Instance = new DependencyTreeGraphHelper();
        }

        [NotNull] private IObserver<ContainerData>[] _observerList =
            Array.Empty<IObserver<ContainerData>>();

        private readonly object _lockObj = new object();

        public IDisposable Subscribe(IObserver<ContainerData> observer)
        {
            lock (_lockObj)
            {
                var list = _observerList.ToList();
                list.Add(observer);
                _observerList = list.ToArray();
            }

            return new Observer(
                this,
                observer
            );
        }

        public void OnNext(
            [NotNull] string id,
            [NotNull] string label,
            [CanBeNull] string parentId,
            [NotNull] IRegistration[] registrations
        )
        {
            var value = new ContainerData(id, label, parentId, registrations);
            foreach (var observer in _observerList)
            {
                observer.OnNext(value);
            }

            _cache.Add(value);
        }

        private class Observer : IDisposable
        {
            private readonly DependencyTreeGraphHelper _self;
            private readonly IObserver<ContainerData> _observer;
            private readonly object _lockObj = new object();

            public Observer(DependencyTreeGraphHelper self, IObserver<ContainerData> observer)
            {
                _self = self;
                _observer = observer;
            }

            public void Dispose()
            {
                lock (_lockObj)
                {
                    lock (_self._lockObj)
                    {
                        if (!_self._observerList.Contains(_observer)) return;
                        var list = _self._observerList.ToList();
                        list.Remove(_observer);
                        _self._observerList = list.ToArray();
                    }
                }
            }
        }

        public readonly struct ContainerData
        {
            [NotNull] public readonly string ID;
            [NotNull] public readonly string Label;
            [CanBeNull] public readonly string ParentId;
            [NotNull] public readonly IRegistration[] Registrations;

            public ContainerData(
                [NotNull] string id,
                [NotNull] string label,
                [CanBeNull] string parentId,
                [NotNull] IRegistration[] registrations)
            {
                ID = id;
                Label = label;
                ParentId = parentId;
                Registrations = registrations;
            }

            public override string ToString()
            {
                return $"[{nameof(ContainerData)}] {nameof(ID)}:{ID}," +
                       $" {nameof(ParentId)}:{ParentId ?? "null"}," +
                       $" {nameof(Registrations)}:{{{string.Join(", ", Registrations.Select(r => $"{r.InstanceType}"))}}}.";
            }
        }
    }
}

#endif