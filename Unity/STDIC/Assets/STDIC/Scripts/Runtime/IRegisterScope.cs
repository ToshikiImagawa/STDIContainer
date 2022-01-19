// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IRegisterScope : IRegisterLazy
    {
        IRegisterLazy AsSingle();
        IRegisterLazy AsTransient();
    }
}