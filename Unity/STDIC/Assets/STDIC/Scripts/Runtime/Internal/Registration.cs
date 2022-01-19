// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC.Internal
{
    internal sealed class Registration : IRegistration
    {
        private readonly IInjector _injector;

        public Registration(
            Type[] injectedTypes,
            Type instanceType,
            ScopeType scopeType,
            IInjector injector)
        {
            InjectedTypes = injectedTypes;
            InstanceType = instanceType;
            ScopeType = scopeType;
            _injector = injector;
        }

        public Type[] InjectedTypes { get; }
        public Type InstanceType { get; }
        public ScopeType ScopeType { get; }

        public object GetInstance(IResolver resolver)
        {
            return _injector.CreateInstance(resolver);
        }
    }
}