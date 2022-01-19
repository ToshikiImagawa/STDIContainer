// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IPool
    {
        System.Type ItemType { get; }
        bool Despawn(object instance);
        void Clear();
    }

    public interface IPool<T> : IDespawnablePool<T>
    {
        T Spawn();
    }
}