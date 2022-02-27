// Copyright (c) 2021 COMCREATE. All rights reserved.

using STDIC.Internal;

namespace STDIC
{
    public interface IRegister
    {
        internal RegisterInfo RegisterInfo { get; }
        internal IRegistration CreateRegistration(IResolver resolver, bool verify);
    }

    public interface IRegisterLazy : IRegister
    {
        IRegister Lazy();
        IRegister NonLazy();
    }

    public interface IRegisterScope : IRegisterLazy
    {
        IRegisterLazy AsSingle();
        IRegisterLazy AsTransient();
    }

    public interface IRegisterType<in TInstance> : IRegisterScope
    {
        IRegisterScope FromNew();
        IRegisterScope FromInstance(TInstance instance);
        IRegisterScope FromFactory(IFactory<TInstance> factory);
    }
}