using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace STDIC.Internal.Registrations
{
    internal sealed class InstanceRegistration : IRegistration
    {
        private readonly RegisterInfo _registerInfo;
        [NotNull] private readonly object _instance;

        public InstanceRegistration(
            RegisterInfo registerInfo,
            object instance,
            bool verify
        )
        {
            if (verify)
            {
                if (instance != null)
                {
                    if (instance.GetType() != registerInfo.InstanceType)
                    {
                        throw new InvalidOperationException(
                            $"Instance must be {registerInfo.InstanceType.FullName}. The instance type is {instance.GetType().FullName}"
                        );
                    }
                }
                else
                {
                    throw new NullReferenceException(
                        $"{registerInfo.InstanceType.FullName} is null."
                    );
                }
            }

            _registerInfo = registerInfo;
            _instance = instance;
        }

        public IEnumerable<Type> ContractTypes => _registerInfo.ContractTypes;
        public Type InstanceType => _registerInfo.InstanceType;
        public IEnumerable<Type> DependentTypes => Array.Empty<Type>();
        public ScopeType ScopeType => _registerInfo.ScopeType;

        public object GetInstance(DiContainer container)
        {
            return _instance;
        }
    }
}