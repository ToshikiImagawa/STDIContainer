// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using STDIC.Internal;
using STDIC.Internal.Reflection;

namespace STDIC
{
    // ReSharper disable once InconsistentNaming
    public class DIContainer : IDisposable
    {
        private DIContainer _rootContainer;
        private DIContainer _parentContainer;
        private readonly IRegistry _registry;
        private readonly IResolver _resolver;
        private readonly ManagedHashTable<IRegistration, Lazy<object>> _sharedInstance;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _disposed;
        private readonly object _lockObj = new object();

        private DIContainer(
            [CanBeNull] string label,
            [NotNull] IRegistry registry,
            [NotNull] IResolver resolver
        )
        {
            _registry = registry;
            _resolver = resolver;
            _rootContainer = this;
            _sharedInstance = new ManagedHashTable<IRegistration, Lazy<object>>();
            Label = label ?? string.Empty;
        }

        private DIContainer(
            [CanBeNull] string label,
            [NotNull] IRegistry registry,
            [NotNull] IResolver resolver,
            [NotNull] DIContainer parentContainer) : this(label, registry, resolver)
        {
            _rootContainer = parentContainer._rootContainer;
            _parentContainer = parentContainer;
            _rootContainer._disposables.Add(this);
        }

#if UNITY_EDITOR
        public string Id { get; } = Guid.NewGuid().ToString();
#endif
        public string Label { get; }

        [NotNull]
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        [NotNull]
        // ReSharper disable once MemberCanBePrivate.Global
        public object Resolve(Type type)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DIContainer));
            var registration = GetRegistration(type);
            return Resolve(registration);
        }

        private static ReflectionResolver _reflectionResolver;

        public static IBuilder CreateBuilder()
        {
            return new Builder(_reflectionResolver ??= new ReflectionResolver());
        }

        public IBuilder CreateChildBuilder()
        {
            return new Builder(_resolver, this);
        }

        private object Resolve([NotNull] IRegistration registration)
        {
            return registration.ScopeType switch
            {
                ScopeType.Transient => registration.GetInstance(this),
                ScopeType.Single => _rootContainer.GetOrCreateSharedInstance(registration, this),
                ScopeType.Cashed => GetOrCreateSharedInstance(registration, this),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private object GetOrCreateSharedInstance([NotNull] IRegistration registration, DIContainer callContainer)
        {
            return _sharedInstance
                .GetOrAddValue(
                    registration,
                    new Lazy<object>(() => registration.GetInstance(callContainer)))
                .Value;
        }

        [NotNull]
        private IRegistration GetRegistration([NotNull] Type contractType)
        {
            var container = this;
            while (container != null)
            {
                if (container._registry.TryGetRegistration(contractType, out var registration)) return registration;
                container = container._parentContainer;
            }

            throw new Exception();
        }

        private bool ContainsRegistration([NotNull] Type contractType)
        {
            var container = this;
            while (container != null)
            {
                if (container._registry.Contains(contractType)) return true;
                container = container._parentContainer;
            }

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~DIContainer()
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

        internal class Builder : IBuilder
        {
            [CanBeNull] private readonly DIContainer _parentContainer;
            private readonly List<IRegister> _registers = new List<IRegister>();
            private readonly IResolver _resolver;

            public Builder(IResolver resolver, DIContainer parentContainer = null)
            {
                _resolver = resolver;
                _parentContainer = parentContainer;
            }

            public IRegisterType<TInstanceType> Register<TInstanceType>(Type[] contractTypes)
            {
                var register = new Register<TInstanceType>(contractTypes);
                _registers.Add(register);
                return register;
            }

            public IRegisterType<TInstanceType> Register<TInjectedType, TInstanceType>()
                where TInstanceType : TInjectedType
            {
                var register = new Register<TInstanceType>(new[] { typeof(TInjectedType) });
                _registers.Add(register);
                return register;
            }

            public IRegisterType<TInstanceType> Register<TInstanceType>()
            {
                var register = new Register<TInstanceType>();
                _registers.Add(register);
                return register;
            }

            public DIContainer Build(
                string label = null,
                bool verify = false
            )
            {
                var registrations = _registers
                    .Select(register => register.CreateRegistration(_resolver, verify))
                    .ToArray();
                _registers.Clear();
                if (verify)
                {
                    var contractTypes = registrations
                        .SelectMany(x => x.ContractTypes).ToArray();
                    var resolutionFailedTypes = registrations
                        .SelectMany(x => x.DependentTypes).ToArray()
                        .Where(
                            requiredType => !contractTypes.Contains(requiredType) &&
                                            !(_parentContainer?.ContainsRegistration(requiredType) ?? false)
                        )
                        .ToArray();

                    if (resolutionFailedTypes.Length > 0)
                    {
                        throw new InvalidOperationException(
                            $"Dependency resolution failed for ({string.Join(", ", resolutionFailedTypes.Select(x => x.FullName))})."
                        );
                    }
                }

                var registry = new Registry(registrations);
                var container = _parentContainer == null
                    ? new DIContainer(label, registry, _resolver)
                    : new DIContainer(label, registry, _resolver, _parentContainer);

#if UNITY_EDITOR
                DependencyTreeGraphHelper.Instance.OnNext(
                    container.Id,
                    container.Label,
                    _parentContainer?.Id,
                    registrations
                );
#endif
                return container;
            }
        }

        public interface IBuilder
        {
            IRegisterType<TInstanceType> Register<TInstanceType>(Type[] contractTypes);
            IRegisterType<TInstanceType> Register<TInjectedType, TInstanceType>() where TInstanceType : TInjectedType;
            IRegisterType<TInstanceType> Register<TInstanceType>();

            DIContainer Build(
                string label = null,
                bool verify = false
            );
        }
    }

    // ReSharper disable once InconsistentNaming
    public static class DIContainer<TResolver> where TResolver : IResolver, new()
    {
        private static readonly TResolver ResolverCache;

        static DIContainer()
        {
            ResolverCache = new TResolver();
        }

        public static DIContainer.IBuilder CreateBuilder()
        {
            return new DIContainer.Builder(ResolverCache);
        }
    }
}