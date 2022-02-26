// Copyright (c) 2022 COMCREATE. All rights reserved.

namespace STDIC
{
    public abstract class Factory<T> : IFactory<T>
    {
        protected DIContainer Container;
        public abstract T Create();

        public void Init(DIContainer container)
        {
            Container = container;
        }
    }
}