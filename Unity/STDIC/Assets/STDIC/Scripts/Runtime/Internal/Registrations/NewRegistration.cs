// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Linq;

namespace STDIC.Internal.Registrations
{
    internal sealed class NewRegistration : IRegistration
    {
        private readonly RegisterInfo _registerInfo;
        private readonly IResolver _resolver;

        public NewRegistration(
            RegisterInfo registerInfo,
            IResolver resolver
        )
        {
            _registerInfo = registerInfo;
            _resolver = resolver;
        }

        public Type[] InjectedTypes => _registerInfo.InjectedTypes;
        public Type InstanceType => _registerInfo.InstanceType;
        public ScopeType ScopeType => _registerInfo.ScopeType;

        public object GetInstance(DiContainer container)
        {
            return _resolver.Constructor(_registerInfo.InstanceType)
                .Invoke(_resolver.GetParameterTypes(_registerInfo.InstanceType).Select(container.Resolve).ToArray());
        }
    }
}