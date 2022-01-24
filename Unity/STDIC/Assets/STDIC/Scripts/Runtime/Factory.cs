// Copyright (c) 2022 COMCREATE. All rights reserved.

namespace STDIC
{
    public abstract class Factory<T> : IFactory<T>
    {
        protected DiContainer Container;
        public abstract T Create();

        public void Init(DiContainer container)
        {
            Container = container;
        }
    }
}