// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using STDIC;

namespace SampleApp
{
    public class GeneratedResolver : IResolver
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
                Constructor = (IConstructor<T>)GeneratedResolverGetConstructorHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class GeneratedResolverGetConstructorHelper
    {
        private static readonly Dictionary<Type, IConstructor> ConstructorMap = new Dictionary<Type, IConstructor>()
        {
            // SampleApp.Domain.Service.ToDoService
            { typeof(SampleApp.Domain.Service.ToDoService), new SampleApp_Domain_Service_ToDoServiceConstructor() },
            // SampleApp.Infrastructure.Repository.ToDoLocalRepository
            { typeof(SampleApp.Infrastructure.Repository.ToDoLocalRepository), new SampleApp_Infrastructure_Repository_ToDoLocalRepositoryConstructor() },
            // SampleApp.Infrastructure.Repository.TodoEventRepository
            { typeof(SampleApp.Infrastructure.Repository.TodoEventRepository), new SampleApp_Infrastructure_Repository_TodoEventRepositoryConstructor() },
            // SampleApp.Infrastructure.Helper.LocalDataBaseHelper
            { typeof(SampleApp.Infrastructure.Helper.LocalDataBaseHelper), new SampleApp_Infrastructure_Helper_LocalDataBaseHelperConstructor() },
        };

        internal static IConstructor GetFormatter(Type t)
        {
            return ConstructorMap.TryGetValue(t, out var constructor) ? constructor : null;
        }
    }

    // SampleApp.Domain.Service.ToDoService
    // ReSharper disable once InconsistentNaming
    internal class SampleApp_Domain_Service_ToDoServiceConstructor :
        IConstructor<SampleApp.Domain.Service.ToDoService>
    {
        private readonly Type[] _parameterTypes;

        public SampleApp_Domain_Service_ToDoServiceConstructor()
        {
            _parameterTypes = new[]
            {
                // SampleApp.Domain.Repository.IToDoRepository
                typeof(SampleApp.Domain.Repository.IToDoRepository),
                // SampleApp.Domain.Repository.ITodoEventRepository
                typeof(SampleApp.Domain.Repository.ITodoEventRepository)
            };
        }

        public Type[] GetParameterTypes() => _parameterTypes;

        public SampleApp.Domain.Service.ToDoService New(object[] parameters)
        {
            return new SampleApp.Domain.Service.ToDoService(
                // SampleApp.Domain.Repository.IToDoRepository
                (SampleApp.Domain.Repository.IToDoRepository)parameters[0],
                // SampleApp.Domain.Repository.ITodoEventRepository
                (SampleApp.Domain.Repository.ITodoEventRepository)parameters[1]
            );
        }
    }

    // SampleApp.Infrastructure.Repository.ToDoLocalRepository
    // ReSharper disable once InconsistentNaming
    internal class SampleApp_Infrastructure_Repository_ToDoLocalRepositoryConstructor :
        IConstructor<SampleApp.Infrastructure.Repository.ToDoLocalRepository>
    {
        private readonly Type[] _parameterTypes;

        public SampleApp_Infrastructure_Repository_ToDoLocalRepositoryConstructor()
        {
            _parameterTypes = new[]
            {
                // SampleApp.Infrastructure.Helper.LocalDataBaseHelper
                typeof(SampleApp.Infrastructure.Helper.LocalDataBaseHelper)
            };
        }

        public Type[] GetParameterTypes() => _parameterTypes;

        public SampleApp.Infrastructure.Repository.ToDoLocalRepository New(object[] parameters)
        {
            return new SampleApp.Infrastructure.Repository.ToDoLocalRepository(
                // SampleApp.Infrastructure.Helper.LocalDataBaseHelper
                (SampleApp.Infrastructure.Helper.LocalDataBaseHelper)parameters[0]
            );
        }
    }

    // SampleApp.Infrastructure.Repository.TodoEventRepository
    // ReSharper disable once InconsistentNaming
    internal class SampleApp_Infrastructure_Repository_TodoEventRepositoryConstructor :
        IConstructor<SampleApp.Infrastructure.Repository.TodoEventRepository>
    {
        private readonly Type[] _parameterTypes;

        public SampleApp_Infrastructure_Repository_TodoEventRepositoryConstructor()
        {
            _parameterTypes = Array.Empty<Type>();
        }

        public Type[] GetParameterTypes() => _parameterTypes;

        public SampleApp.Infrastructure.Repository.TodoEventRepository New(object[] parameters)
        {
            return new SampleApp.Infrastructure.Repository.TodoEventRepository();
        }
    }

    // SampleApp.Infrastructure.Helper.LocalDataBaseHelper
    // ReSharper disable once InconsistentNaming
    internal class SampleApp_Infrastructure_Helper_LocalDataBaseHelperConstructor :
        IConstructor<SampleApp.Infrastructure.Helper.LocalDataBaseHelper>
    {
        private readonly Type[] _parameterTypes;

        public SampleApp_Infrastructure_Helper_LocalDataBaseHelperConstructor()
        {
            _parameterTypes = Array.Empty<Type>();
        }

        public Type[] GetParameterTypes() => _parameterTypes;

        public SampleApp.Infrastructure.Helper.LocalDataBaseHelper New(object[] parameters)
        {
            return new SampleApp.Infrastructure.Helper.LocalDataBaseHelper();
        }
    }
}