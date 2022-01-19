// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC.Internal
{
    internal interface IResolver
    {
        object Resolve(Type type);
    }
}