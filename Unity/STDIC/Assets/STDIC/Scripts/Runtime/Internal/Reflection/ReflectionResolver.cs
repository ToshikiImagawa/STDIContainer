// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Linq;
using System.Reflection;

namespace STDIC.Internal.Reflection
{
    internal class ReflectionResolver : IResolver
    {
        private readonly TypeKeyHashTable<ConstructorInfo> _constructorInfoHashTable =
            new TypeKeyHashTable<ConstructorInfo>();

        public Type[] GetParameterTypes(Type instanceType)
        {
            return GetConstructorInfo(instanceType).GetParameters().Select(info => info.ParameterType).ToArray();
        }

        public Func<object[], object> Constructor(Type instanceType)
        {
            return GetConstructorInfo(instanceType).Invoke;
        }

        private ConstructorInfo GetConstructorInfo(Type instanceType)
        {
            if (_constructorInfoHashTable.TryGetValue(instanceType, out var cacheConstructorInfo))
            {
                return cacheConstructorInfo;
            }

            var constructorInfo = instanceType.GetAllInjectConstructors().FirstOrDefault() ?? instanceType.GetDefaultConstructors();
            _constructorInfoHashTable.TryAdd(instanceType, constructorInfo);
            return constructorInfo;
        }
    }
}