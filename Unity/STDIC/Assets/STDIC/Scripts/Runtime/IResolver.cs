// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IResolver
    {
        IConstructor<T> GetConstructor<T>();
    }
}