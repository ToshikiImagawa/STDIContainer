// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using SampleApp.Model;

namespace SampleApp.Domain.Repository
{
    public interface ITodoEventRepository
    {
        void Updated(ToDo todo);
        void Added(ToDo todo);
        void Removed(ToDo todo);
        IObservable<ToDo> OnUpdatedToDo { get; }
        IObservable<ToDo> OnAddedToDo { get; }
        IObservable<ToDo> OnRemovedToDo { get; }
    }
}