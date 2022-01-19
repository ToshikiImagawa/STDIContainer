// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC
{
    public interface IDespawnablePool<in TValue> : IPool
    {
        void Despawn(TValue item);
    }
}