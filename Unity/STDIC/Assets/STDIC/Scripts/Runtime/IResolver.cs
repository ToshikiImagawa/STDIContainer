// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;

namespace STDIC
{
    public interface IResolver
    {
        IConstructor<T> GetConstructor<T>();

        IEnumerable<Type> HasInjectConstructorTypes { get; }
    }
}