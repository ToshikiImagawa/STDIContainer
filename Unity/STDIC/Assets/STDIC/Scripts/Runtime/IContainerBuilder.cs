// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IContainerBuilder
    {
        IRegisterType<TInstanceType> Register<TInstanceType>();
        IRegisterType<T> Register<T, TInstanceType>() where TInstanceType : T;
    }
}