// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using STDIC.Internal.Registrations;

namespace STDIC.Internal
{
    internal class Register<TInstance> : IRegisterType<TInstance>
    {
        private Lazy<IRegistration> _registrationLazy;
        private RegisterInfo _registerInfo;

        IRegistration IRegister.Registration => _registrationLazy.Value;

        RegisterInfo IRegister.RegisterInfo => _registerInfo;

        private readonly IResolver _resolver;

        public Register(IResolver resolver, Type[] injectedTypes)
        {
            _resolver = resolver;
            _registerInfo = new RegisterInfo(
                injectedTypes,
                typeof(TInstance),
                ScopeType.Transient,
                true
            );
            _registrationLazy = new Lazy<IRegistration>(() => new NewRegistration(_registerInfo, _resolver));
        }

        public Register(IResolver resolver)
        {
            _resolver = resolver;
            _registerInfo = new RegisterInfo(
                new[] { typeof(TInstance) },
                typeof(TInstance),
                ScopeType.Transient,
                true
            );
            _registrationLazy = new Lazy<IRegistration>(() => new NewRegistration(_registerInfo, _resolver));
        }

        public IRegister Lazy()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.InjectedTypes,
                _registerInfo.InstanceType,
                _registerInfo.ScopeType,
                true
            );
            return this;
        }

        public IRegister NonLazy()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.InjectedTypes,
                _registerInfo.InstanceType,
                _registerInfo.ScopeType,
                false
            );
            return this;
        }

        public IRegisterLazy AsSingle()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.InjectedTypes,
                _registerInfo.InstanceType,
                ScopeType.Single,
                _registerInfo.IsLazy
            );
            return this;
        }

        public IRegisterLazy AsTransient()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.InjectedTypes,
                _registerInfo.InstanceType,
                ScopeType.Transient,
                _registerInfo.IsLazy
            );
            return this;
        }

        public IRegisterScope FromNew()
        {
            _registrationLazy = new Lazy<IRegistration>(() => new NewRegistration(_registerInfo, _resolver));
            return this;
        }

        public IRegisterScope FromInstance(TInstance instance)
        {
            _registrationLazy = new Lazy<IRegistration>(() => new InstanceRegistration(_registerInfo, instance));
            return this;
        }

        public IRegisterScope FromFactory(IFactory<TInstance> factory)
        {
            _registrationLazy =
                new Lazy<IRegistration>(() => new FactoryRegistration<TInstance>(_registerInfo, factory));
            return this;
        }
    }
}