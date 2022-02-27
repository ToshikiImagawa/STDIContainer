// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace STDIC.Internal.Registrations
{
    internal sealed class NewRegistration<TInstanceType> : IRegistration
    {
        private readonly RegisterInfo _registerInfo;
        private readonly Lazy<IConstructor<TInstanceType>> _constructorLazy;

        public NewRegistration(
            RegisterInfo registerInfo,
            IResolver resolver,
            bool verify
        )
        {
            if (verify)
            {
                if (!resolver.HasInjectConstructorTypes.Contains(registerInfo.InstanceType))
                {
                    throw new InvalidOperationException(
                        $"{registerInfo.InstanceType.FullName} is not found inject constructor."
                    );
                }

                DependentTypes = resolver.GetConstructor<TInstanceType>().GetParameterTypes();
            }
            else
            {
                DependentTypes = Array.Empty<Type>();
            }

            _registerInfo = registerInfo;
            _constructorLazy = new Lazy<IConstructor<TInstanceType>>(resolver.GetConstructor<TInstanceType>);
        }

        public IEnumerable<Type> ContractTypes => _registerInfo.ContractTypes;
        public Type InstanceType => _registerInfo.InstanceType;
        public IEnumerable<Type> DependentTypes { get; }
        public ScopeType ScopeType => _registerInfo.ScopeType;

        public object GetInstance(DIContainer container)
        {
            var constructor = _constructorLazy.Value;
            return constructor.New(constructor.GetParameterTypes().Select(container.Resolve).ToArray());
        }
    }
}