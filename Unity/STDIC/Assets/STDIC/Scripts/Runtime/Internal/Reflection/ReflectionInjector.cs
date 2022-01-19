// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace STDIC.Internal.Reflection
{
    internal class ReflectionInjector : IInjector
    {
        [NotNull] private readonly ConstructorInfo[] _constructorInfos;

        public ReflectionInjector([NotNull] Type injectType)
        {
            _constructorInfos = injectType.GetAllInjectConstructors();
        }

        public void Inject(object instance, IResolver resolver)
        {
            // todo: constructor injection only
        }

        public object CreateInstance(IResolver resolver)
        {
            var constructorInfo = _constructorInfos.First();
            var instance = constructorInfo.Invoke(constructorInfo
                .GetParameters()
                .Select(info => resolver.Resolve(info.ParameterType))
                .ToArray());
            Inject(instance, resolver);
            return instance;
        }
    }
}