// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Linq;

namespace STDIC.Internal.Reflection
{
    internal class ReflectionResolver : IResolver
    {
        public IConstructor<T> GetConstructor<T>()
        {
            return ConstructorCache<T>.Constructor;
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
                                      instanceType.GetDefaultConstructors() ??
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