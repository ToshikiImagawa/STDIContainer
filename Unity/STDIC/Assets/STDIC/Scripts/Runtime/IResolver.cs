// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;

namespace STDIC
{
    public interface IResolver
    {
        Type[] GetParameterTypes(Type instanceType);
        Func<object[], object> Constructor(Type instanceType);
    }
}