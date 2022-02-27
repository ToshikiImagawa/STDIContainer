// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using JetBrains.Annotations;
using STDIC.Internal.Registrations;

namespace STDIC.Internal
{
    internal class Register<TInstanceType> : IRegisterType<TInstanceType>
    {
        [NotNull] private Func<IResolver, RegisterInfo, bool, IRegistration> _registrationFactory;
        private RegisterInfo _registerInfo;

        RegisterInfo IRegister.RegisterInfo => _registerInfo;

        IRegistration IRegister.CreateRegistration(IResolver resolver, bool verify)
        {
            return _registrationFactory(resolver, _registerInfo, verify);
        }

        public Register(Type[] contractTypes)
        {
            _registerInfo = new RegisterInfo(
                contractTypes,
                typeof(TInstanceType),
                ScopeType.Transient,
                true
            );
            _registrationFactory = (resolver, registerInfo, verify) =>
                new NewRegistration<TInstanceType>(registerInfo, resolver, verify);
        }

        public Register()
        {
            _registerInfo = new RegisterInfo(
                new[] { typeof(TInstanceType) },
                typeof(TInstanceType),
                ScopeType.Transient,
                true
            );
            _registrationFactory = (resolver, registerInfo, verify) =>
                new NewRegistration<TInstanceType>(
                    _registerInfo,
                    resolver,
                    verify
                );
        }

        public IRegister Lazy()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.ContractTypes,
                _registerInfo.InstanceType,
                _registerInfo.ScopeType,
                true
            );
            return this;
        }

        public IRegister NonLazy()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.ContractTypes,
                _registerInfo.InstanceType,
                _registerInfo.ScopeType,
                false
            );
            return this;
        }

        public IRegisterLazy AsSingle()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.ContractTypes,
                _registerInfo.InstanceType,
                ScopeType.Single,
                _registerInfo.IsLazy
            );
            return this;
        }

        public IRegisterLazy AsTransient()
        {
            _registerInfo = new RegisterInfo(
                _registerInfo.ContractTypes,
                _registerInfo.InstanceType,
                ScopeType.Transient,
                _registerInfo.IsLazy
            );
            return this;
        }

        public IRegisterScope FromNew()
        {
            _registrationFactory = (resolver, registerInfo, verify) =>
                new NewRegistration<TInstanceType>(
                    _registerInfo,
                    resolver,
                    verify
                );
            return this;
        }

        public IRegisterScope FromInstance(TInstanceType instance)
        {
            _registrationFactory = (resolver, registerInfo, verify) =>
                new InstanceRegistration(
                    _registerInfo,
                    instance,
                    verify
                );
            return this;
        }

        public IRegisterScope FromFactory(IFactory<TInstanceType> factory)
        {
            _registrationFactory = (resolver, registerInfo, verify) =>
                new FactoryRegistration<TInstanceType>(
                    _registerInfo,
                    factory,
                    verify
                );
            return this;
        }
    }
}