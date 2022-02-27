// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using JetBrains.Annotations;
using SampleApp.Domain.Service;
using SampleApp.Model;
using SampleApp.Utils;

namespace SampleApp.ViewModel
{
    public class ToDoViewModel : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        [NotNull] private readonly IToDoService _toDoService;
        private readonly Subject<ToDo> _todo = new Subject<ToDo>();

        public int Id => Current.Id;

        public ToDoStatus Status
        {
            get => Current.Status;
            set => _toDoService.UpdateStatus(Current.Id, value);
        }

        public string Message => Current.Message;
        public IObservable<ToDo> ToDo => _todo;

        private ToDo Current { get; set; }

        public ToDoViewModel(ToDo todo, [NotNull] IToDoService toDoService)
        {
            Current = todo;
            _toDoService = toDoService;
            _compositeDisposable = new CompositeDisposable()
                .Add(toDoService.OnUpdateToDo.Subscribe(OnUpdate))
                .Add(_todo);
        }

        public void Delete()
        {
            _toDoService.Delete(Id);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        private void OnUpdate(ToDo todo)
        {
            if (Id != todo.Id) return;
            Current = todo;
            _todo.OnNext(todo);
        }
    }
}