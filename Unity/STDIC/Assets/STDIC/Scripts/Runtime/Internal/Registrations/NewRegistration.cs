// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Linq;

namespace STDIC.Internal.Registrations
{
    internal sealed class NewRegistration<TInstanceType> : IRegistration
    {
        private readonly RegisterInfo _registerInfo;
        private readonly Lazy<IConstructor<TInstanceType>> _constructorLazy;

        public NewRegistration(
            RegisterInfo registerInfo,
            IResolver resolver
        )
        {
            _registerInfo = registerInfo;
            _constructorLazy = new Lazy<IConstructor<TInstanceType>>(resolver.GetConstructor<TInstanceType>);
        }

        public Type[] InjectedTypes => _registerInfo.InjectedTypes;
        public Type InstanceType => _registerInfo.InstanceType;
        public ScopeType ScopeType => _registerInfo.ScopeType;

        public object GetInstance(DiContainer container)
        {
            var constructor = _constructorLazy.Value;
            return constructor.New(constructor.GetParameterTypes().Select(container.Resolve).ToArray());
        }
    }
}