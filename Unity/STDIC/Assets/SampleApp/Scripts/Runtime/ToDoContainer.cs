// Copyright (c) 2022 COMCREATE. All rights reserved.

using SampleApp.Domain.Repository;
using SampleApp.Domain.Service;
using SampleApp.Infrastructure.Helper;
using SampleApp.Infrastructure.Repository;
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

        public readonly DiContainer Container;

        private ToDoContainer()
        {
            var builder = DiContainer.CreateBuilder<GeneratedResolver>();
            builder.Register<IToDoService, ToDoService>().FromNew();
            builder.Register<IToDoRepository, ToDoLocalRepository>().FromNew().AsSingle();
            builder.Register<ITodoEventRepository, TodoEventRepository>().FromNew().AsSingle();
            builder.Register<LocalDataBaseHelper>().FromNew().AsSingle();
            Container = builder.Build();
        }
    }
}