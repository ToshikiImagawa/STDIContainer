// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC.Internal
{
    internal readonly struct RegisterInfo
    {
        public RegisterInfo(
            Type[] injectedTypes,
            Type instanceType,
            ScopeType scopeType,
            bool isLazy)
        {
            InjectedTypes = injectedTypes;
            InstanceType = instanceType;
            ScopeType = scopeType;
            IsLazy = isLazy;
        }

        public Type[] InjectedTypes { get; }
        public Type InstanceType { get; }
        public ScopeType ScopeType { get; }
        public bool IsLazy { get; }
    }
}