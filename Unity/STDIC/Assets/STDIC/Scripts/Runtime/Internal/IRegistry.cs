// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC.Internal
{
    internal interface IRegistry
    {
        bool TryGetRegistration(Type contractType, out IRegistration registration);
        bool Contains(Type contractType);
    }
}