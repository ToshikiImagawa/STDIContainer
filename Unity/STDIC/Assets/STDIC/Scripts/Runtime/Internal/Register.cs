// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using STDIC.Internal.Registrations;

namespace STDIC.Internal
{
    internal class Register<TInstanceType> : IRegisterType<TInstanceType>
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
                typeof(TInstanceType),
                ScopeType.Transient,
                true
            );
            _registrationLazy =
                new Lazy<IRegistration>(() => new NewRegistration<TInstanceType>(_registerInfo, _resolver));
        }

        public Register(IResolver resolver)
        {
            _resolver = resolver;
            _registerInfo = new RegisterInfo(
                new[] { typeof(TInstanceType) },
                typeof(TInstanceType),
                ScopeType.Transient,
                true
            );
            _registrationLazy =
                new Lazy<IRegistration>(() => new NewRegistration<TInstanceType>(_registerInfo, _resolver));
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
            _registrationLazy =
                new Lazy<IRegistration>(() => new NewRegistration<TInstanceType>(_registerInfo, _resolver));
            return this;
        }

        public IRegisterScope FromInstance(TInstanceType instance)
        {
            _registrationLazy = new Lazy<IRegistration>(() => new InstanceRegistration(_registerInfo, instance));
            return this;
        }

        public IRegisterScope FromFactory(IFactory<TInstanceType> factory)
        {
            _registrationLazy =
                new Lazy<IRegistration>(() => new FactoryRegistration<TInstanceType>(_registerInfo, factory));
            return this;
        }
    }
}