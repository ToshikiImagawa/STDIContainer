// Copyright (c) 2022 COMCREATE. All rights reserved.

using SampleApp.Domain.Repository;
using SampleApp.Domain.Service;
using SampleApp.Infrastructure.Helper;
using SampleApp.Infrastructure.Repository;
using SampleApp.Utils;
using STDIC;

namespace SampleApp
{
    public class ToDoContainer
    {
        public static readonly ToDoContainer Instance;

        static ToDoContainer()
        {
            Instance = new ToDoContainer();
        }

        public readonly DIContainer Container = DIContainer<GeneratedResolver>
            .CreateBuilder()
            .Also(it =>
            {
                it.Register<LocalDataBaseHelper>()
                    .FromNew()
                    .AsSingle();
            }).Build("HelperContainer", true)
            .CreateChildBuilder()
            .Also(it =>
            {
                it.Register<ITodoEventRepository, TodoEventRepository>()
                    .FromInstance(new TodoEventRepository())
                    .AsSingle();
                it.Register<IToDoRepository, ToDoLocalRepository>()
                    .FromNew()
                    .AsSingle();
            })
            .Build("RepositoryContainer", true)
            .CreateChildBuilder()
            .Also(it =>
            {
                it.Register<IToDoService, ToDoService>()
                    .FromNew();
            }).Build(verify: true);
    }
}