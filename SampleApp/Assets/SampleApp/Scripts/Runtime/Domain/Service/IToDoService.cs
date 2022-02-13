// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Threading.Tasks;
using SampleApp.Model;

namespace SampleApp.Domain.Service
{
    public interface IToDoService
    {
        public Task<ToDo> Create(string message);
        public void UpdateStatus(int id, ToDoStatus status);
        public void Delete(int id);
        public Task<ToDo[]> ToDoList { get; }
        public IObservable<ToDo> OnUpdateToDo { get; }
        public IObservable<ToDo> OnAddedToDo { get; }
        public IObservable<ToDo> OnRemovedToDo { get; }
    }
}