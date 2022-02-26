using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace STDIC.Internal.Registrations
{
    internal sealed class FactoryRegistration<T> : IRegistration
    {
        private readonly RegisterInfo _registerInfo;
        [NotNull] private readonly IFactory<T> _factory;
        private bool _isFactoryInitialized;

        public FactoryRegistration(
            RegisterInfo registerInfo,
            [NotNull] IFactory<T> factory,
            bool verify
        )
        {
            if (verify)
            {
                // pass through
            }

            _registerInfo = registerInfo;
            _factory = factory;
        }

        public IEnumerable<Type> ContractTypes => _registerInfo.ContractTypes;
        public Type InstanceType => _registerInfo.InstanceType;
        public IEnumerable<Type> DependentTypes => Array.Empty<Type>();
        public ScopeType ScopeType => _registerInfo.ScopeType;

        public object GetInstance(DIContainer container)
        {
            if (_isFactoryInitialized) return _factory.Create();
            _isFactoryInitialized = true;
            _factory.Init(container);
            return _factory.Create();
        }
    }
}