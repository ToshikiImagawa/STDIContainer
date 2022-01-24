// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using STDIC.Internal;
using STDIC.Internal.Reflection;

namespace STDIC
{
    public class DiContainer : IDisposable
    {
        private DiContainer _rootContainer;
        private DiContainer _parentContainer;
        private readonly IRegistry _registry;
        private readonly ManagedHashTable<IRegistration, Lazy<object>> _sharedInstance;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _disposed;
        private readonly object _lockObj = new object();

        private DiContainer(
            [NotNull] IRegistry registry
        )
        {
            _registry = registry;
            _rootContainer = this;
            _sharedInstance = new ManagedHashTable<IRegistration, Lazy<object>>();
        }

        private DiContainer(
            [NotNull] IRegistry registry,
            [NotNull] DiContainer parentContainer
        ) : this(registry)
        {
            _rootContainer = parentContainer._rootContainer;
            _parentContainer = parentContainer;
            _rootContainer._disposables.Add(this);
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

        public static IBuilder CreateBuilder<TResolver>() where TResolver : IResolver, new()
        {
            return new Builder<TResolver>();
        }

        public static IBuilder CreateBuilder()
        {
            return new Builder<ReflectionResolver>();
        }

        public IBuilder CreateChildBuilder<TResolver>() where TResolver : IResolver, new()
        {
            return new Builder<TResolver>(_parentContainer);
        }

        public IBuilder CreateChildBuilder()
        {
            return new Builder<ReflectionResolver>(_parentContainer);
        }

        private object Resolve([NotNull] IRegistration registration)
        {
            switch (registration.ScopeType)
            {
                case ScopeType.Transient:
                    return registration.GetInstance(this);
                case ScopeType.Single:
                    return _rootContainer.GetOrCreateSharedInstance(registration);
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
            lock (_lockObj)
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

        private class Builder<TResolver> : IBuilder where TResolver : IResolver, new()
        {
            [NotNull] private readonly IResolver _resolver;
            [CanBeNull] private readonly DiContainer _parentContainer;
            private readonly List<IRegister> _registers = new List<IRegister>();

            public Builder(DiContainer parentContainer)
            {
                _resolver = new TResolver();
                _parentContainer = parentContainer;
            }

            public Builder()
            {
                _resolver = new TResolver();
                _parentContainer = null;
            }

            public IRegisterType<TInstanceType> Register<TInstanceType>(Type[] injectedTypes)
            {
                var register = new Register<TInstanceType>(_resolver, injectedTypes);
                _registers.Add(register);
                return register;
            }

            public IRegisterType<TInstanceType> Register<TInjectedType, TInstanceType>()
                where TInstanceType : TInjectedType
            {
                var register = new Register<TInstanceType>(_resolver, new[] { typeof(TInjectedType) });
                _registers.Add(register);
                return register;
            }

            public IRegisterType<TInstanceType> Register<TInstanceType>()
            {
                var register = new Register<TInstanceType>(_resolver);
                _registers.Add(register);
                return register;
            }

            public DiContainer Build()
            {
                var registry = new Registry(_registers.ToArray().Select(register => register.Registration));
                _registers.Clear();
                return _parentContainer == null
                    ? new DiContainer(registry)
                    : new DiContainer(registry, _parentContainer);
            }
        }

        public interface IBuilder
        {
            IRegisterType<TInstanceType> Register<TInstanceType>(Type[] injectedTypes);
            IRegisterType<TInstanceType> Register<TInjectedType, TInstanceType>() where TInstanceType : TInjectedType;
            IRegisterType<TInstanceType> Register<TInstanceType>();
            DiContainer Build();
        }
    }
}