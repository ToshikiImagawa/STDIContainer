// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IRegisterLazy : IRegister
    {
        IRegister Lazy();
        IRegister NonLazy();
    }
}