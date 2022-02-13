// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;

namespace STDIC.Internal
{
    internal interface IRegistry
    {
        IEnumerable<IRegistration> Registrations { get; }
        bool TryGetRegistration(Type contractType, out IRegistration registration);
        bool Contains(Type contractType);
    }
}