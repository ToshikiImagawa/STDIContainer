// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using JetBrains.Annotations;
using STDIC.Internal;

namespace STDIC
{
    public class DiContainer : IDisposable, IResolver
    {
        private DiContainer _rootContainer;
        private DiContainer _parentContainer;
        private readonly Func<Type, IInjector> _injectorFactory;
        private readonly IRegistry _registry;
        private readonly ManagedHashTable<IRegistration, Lazy<object>> _sharedInstance;

        private readonly TypeKeyHashTable<IInjector> _injectorHashTable =
            new TypeKeyHashTable<IInjector>(Array.Empty<(Type, IInjector)>());

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _disposed;

        internal DiContainer(
            [NotNull] IRegistry registry,
            [NotNull] Func<Type, IInjector> injectorFactory)
        {
            _registry = registry;
            _injectorFactory = injectorFactory;
            _sharedInstance = new ManagedHashTable<IRegistration, Lazy<object>>();
        }

        internal DiContainer(
            [NotNull] IRegistry registry,
            [NotNull] Func<Type, IInjector> injectorFactory,
            [NotNull] DiContainer rootContainer,
            [CanBeNull] DiContainer parentContainer = null) : this(registry, injectorFactory)
        {
            _rootContainer = rootContainer;
            _parentContainer = parentContainer;
            _rootContainer._disposables.Add(this);
            _injectorHashTable = rootContainer._injectorHashTable;
        }

        [NotNull]
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public object Resolve(Type type)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DiContainer));
            var registration = GetRegistration(type);
            switch (registration.ScopeType)
            {
                case ScopeType.Transient:
                    return registration.GetInstance(this);
                case ScopeType.Single:
                    if (_parentContainer is null)
                    {
                        _rootContainer.Resolve(registration);
                    }

                    var sharedInstance = _rootContainer?._sharedInstance ?? _sharedInstance;
                    return sharedInstance
                        .GetOrAddValue(registration, new Lazy<object>(() => registration.GetInstance(this)))
                        .Value;
                case ScopeType.Cashed:
                    return _sharedInstance
                        .GetOrAddValue(registration, new Lazy<object>(() => registration.GetInstance(this)))
                        .Value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object Resolve([NotNull] IRegistration registration)
        {
            switch (registration.ScopeType)
            {
                case ScopeType.Transient:
                    return registration.GetInstance(this);
                case ScopeType.Single:
                    if (_rootContainer == null)
                        return GetOrCreateSharedInstance(registration);
                    if (_parentContainer is null)
                        return _rootContainer.Resolve(registration);
                    return !_registry.Contains(registration.InstanceType)
                        ? _parentContainer.Resolve(registration)
                        : GetOrCreateSharedInstance(registration);
                case ScopeType.Cashed:
                    return GetOrCreateSharedInstance(registration);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object GetOrCreateSharedInstance([NotNull] IRegistration registration)
        {
            return _sharedInstance
                .GetOrAddValue(
                    registration,
                    new Lazy<object>(() => registration.GetInstance(this)))
                .Value;
        }

        public void Inject(object instance)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DiContainer));
            GetInjector(instance.GetType()).Inject(instance, this);
        }

        private IInjector GetInjector(Type instanceType)
        {
            return _injectorHashTable.GetOrAddValue(
                instanceType,
                type => _injectorFactory.Invoke(type)
            );
        }

        [NotNull]
        private IRegistration GetRegistration([NotNull] Type type)
        {
            var container = this;
            while (container != null)
            {
                if (container._registry.TryGetRegistration(type, out var registration)) return registration;
                container = container._parentContainer;
            }

            throw new Exception();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~DiContainer()
        {
            Dispose(false);
        }

        private void Dispose(bool isDisposing)
        {
            lock (this)
            {
                if (_disposed) return;
                _disposed = true;
                if (!isDisposing) return;
                _disposables.Dispose();
                _parentContainer = null;
                _rootContainer = null;
                GC.SuppressFinalize(this);
            }
        }
    }
}