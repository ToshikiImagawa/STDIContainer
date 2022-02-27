// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace STDIC.Internal.Reflection
{
    internal class ReflectionResolver : IResolver
    {
        public IConstructor<T> GetConstructor<T>()
        {
            return ConstructorCache<T>.Constructor;
        }

        public IEnumerable<Type> HasInjectConstructorTypes
        {
            get
            {
                foreach (var hasInjectConstructorType in HasInjectConstructorTypePairs)
                {
                    var (instanceType, _) = hasInjectConstructorType;
                    yield return instanceType;
                }
            }
        }

        private (Type, ConstructorInfo)[] _hasInjectConstructorTypePairs;

        private IEnumerable<(Type, ConstructorInfo)> HasInjectConstructorTypePairs
        {
            get
            {
                if (_hasInjectConstructorTypePairs != null) return _hasInjectConstructorTypePairs;
                _hasInjectConstructorTypePairs = (
                    from type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes())
                    let constructorInfo = type.GetAllInjectConstructors().FirstOrDefault()
                    where constructorInfo != null
                    select (type, constructorInfo)
                ).ToArray();
                return _hasInjectConstructorTypePairs;
            }
        }

        private static class ConstructorCache<T>
        {
            public static readonly IConstructor<T> Constructor;

            static ConstructorCache()
            {
                Constructor = new ReflectionConstructor<T>();
            }
        }

        private class ReflectionConstructor<T> : IConstructor<T>
        {
            private readonly Type[] _parameterTypes;
            private readonly Func<object[], object> _constructor;

            public ReflectionConstructor()
            {
                var instanceType = typeof(T);
                var constructorInfo = instanceType.GetAllInjectConstructors().FirstOrDefault() ??
                                      throw new InvalidOperationException(
                                          $"{instanceType.FullName} is not found inject constructor."
                                      );
                _parameterTypes = constructorInfo.GetParameters().Select(info => info.ParameterType).ToArray();
                _constructor = constructorInfo.Invoke;
            }

            public Type[] GetParameterTypes()
            {
                return _parameterTypes;
            }

            public T New(object[] parameters)
            {
                return (T)_constructor(parameters);
            }
        }
    }
}