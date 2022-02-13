// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC.Internal
{
    internal readonly struct RegisterInfo
    {
        public RegisterInfo(
            Type[] contractTypes,
            Type instanceType,
            ScopeType scopeType,
            bool isLazy)
        {
            ContractTypes = contractTypes;
            InstanceType = instanceType;
            ScopeType = scopeType;
            IsLazy = isLazy;
        }

        public Type[] ContractTypes { get; }
        public Type InstanceType { get; }
        public ScopeType ScopeType { get; }
        public bool IsLazy { get; }
    }
}