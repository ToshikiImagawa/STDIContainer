using System;
using JetBrains.Annotations;

namespace STDIC.Internal.Registrations
{
    internal sealed class InstanceRegistration : IRegistration
    {
        private readonly RegisterInfo _registerInfo;
        [NotNull] private readonly object _instance;

        public InstanceRegistration(RegisterInfo registerInfo, object instance)
        {
            _registerInfo = registerInfo;
            _instance = instance;
        }

        public Type[] InjectedTypes => _registerInfo.InjectedTypes;
        public Type InstanceType => _registerInfo.InstanceType;
        public ScopeType ScopeType => _registerInfo.ScopeType;

        public object GetInstance(DiContainer container)
        {
            return _instance;
        }
    }
}