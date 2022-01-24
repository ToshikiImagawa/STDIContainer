using System;
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
            [NotNull] IFactory<T> factory
        )
        {
            _registerInfo = registerInfo;
            _factory = factory;
        }

        public Type[] InjectedTypes => _registerInfo.InjectedTypes;
        public Type InstanceType => _registerInfo.InstanceType;
        public ScopeType ScopeType => _registerInfo.ScopeType;

        public object GetInstance(DiContainer container)
        {
            if (_isFactoryInitialized) return _factory.Create();
            _isFactoryInitialized = true;
            _factory.Init(container);
            return _factory.Create();
        }
    }
}