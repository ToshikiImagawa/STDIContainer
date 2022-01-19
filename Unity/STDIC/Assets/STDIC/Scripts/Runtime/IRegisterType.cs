// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IRegisterType<in T> : IRegisterScope
    {
        IRegisterScope FromNew();
        IRegisterScope FromInstance(T instance);
    }
}