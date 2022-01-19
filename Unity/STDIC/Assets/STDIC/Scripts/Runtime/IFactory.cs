// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IFactory
    {
    }

    public interface IFactory<out T> : IFactory
    {
        T Create();
    }
}