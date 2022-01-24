// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC.Internal
{
    internal interface IRegistration
    {
        public Type[] InjectedTypes { get; }
        public Type InstanceType { get; }
        public ScopeType ScopeType { get; }
        public object GetInstance(DiContainer container);
    }
}