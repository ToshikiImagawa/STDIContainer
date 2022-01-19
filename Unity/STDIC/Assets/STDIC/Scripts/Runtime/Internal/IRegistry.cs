// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC.Internal
{
    internal interface IRegistry
    {
        bool TryGetRegistration(Type injectedType, out IRegistration registration);
        bool Contains(Type injectedType);
    }
}