// Copyright (c) 2021 COMCREATE. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace STDIC.Internal.Reflection
{
    internal static class ReflectionUtil
    {
        private const BindingFlags ALL_INSTANCE_BINDING_FLAGS =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.FlattenHierarchy;

        public static ConstructorInfo[] GetAllInjectConstructors([NotNull] this Type self)
        {
            return self.GetConstructors()
                .Where(info => info.GetCustomAttribute<InjectAttribute>() != null)
                .ToArray();
        }

        public static ConstructorInfo[] GetConstructors([NotNull] this Type self)
        {
            return self.GetConstructors(ALL_INSTANCE_BINDING_FLAGS);
        }

        public static bool IsImplemented(Type instanceType, Type implementedType)
        {
            if (instanceType == implementedType) return true;
            if (implementedType.IsInterface)
            {
                return IsFindInterface(instanceType, implementedType);
            }

            if (implementedType.IsClass)
            {
                return IsFindAbstract(instanceType, implementedType);
            }

            return false;
        }

        private static bool IsFindInterface(Type instanceType, Type findInterfaceType)
        {
            var interfaceTypes = instanceType.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == findInterfaceType)
                    return true;
                if (interfaceType == findInterfaceType) return true;
            }

            return false;
        }

        private static bool IsFindAbstract(Type instanceType, Type findAbstractType)
        {
            var baseType = instanceType.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == findAbstractType) return true;
                if (baseType == findAbstractType) return true;
                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}