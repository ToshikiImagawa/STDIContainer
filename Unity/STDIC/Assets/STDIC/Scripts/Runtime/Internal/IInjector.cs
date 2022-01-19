// Copyright (c) 2021 COMCREATE. All rights reserved.

namespace STDIC.Internal
{
    internal interface IInjector
    {
        void Inject(object instance, IResolver resolver);
        object CreateInstance(IResolver resolver);
    }
}