// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IFactory
    {
        void Init(DIContainer container);
    }

    public interface IFactory<out T> : IFactory
    {
        T Create();
    }
}