// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SampleApp.Domain.Service;
using SampleApp.Model;
using SampleApp.Utils;

namespace SampleApp.ViewModel
{
    public class ToDoListViewModel : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable;
        [NotNull] private readonly IToDoService _toDoService;
        private readonly Subject<ToDoViewModel> _addTodo = new Subject<ToDoViewModel>();
        private readonly Subject<ToDoViewModel> _removeTodo = new Subject<ToDoViewModel>();
        private readonly List<ToDoViewModel> _list = new List<ToDoViewModel>();
        private bool _disposed;
        private readonly object _lockObj = new object();

        public Task<ToDoViewModel[]> Current => GetCurrent();
        public IObservable<ToDoViewModel> OnAdd => _addTodo;
        public IObservable<ToDoViewModel> OnRemove => _removeTodo;

        public ToDoListViewModel([NotNull] IToDoService toDoService)
        {
            _toDoService = toDoService;
            _compositeDisposable = new CompositeDisposable()
                .Add(toDoService.OnAddedToDo.Subscribe(OnNextAdded))
                .Add(toDoService.OnRemovedToDo.Subscribe(OnNextRemoved))
                .Add(_addTodo)
                .Add(_removeTodo);
        }

        public async Task<ToDoViewModel> Create(string message)
        {
            var todo = await _toDoService.Create(message);
            lock (_lockObj)
            {
                var added = _list.FirstOrDefault(model => model.Id == todo.Id);
                if (added != null) return added;
                var model = new ToDoViewModel(todo, _toDoService);
                _list.Add(model);
                _addTodo.OnNext(model);
                return model;
            }
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
            ToDoViewModel[] list;
            lock (_lockObj)
            {
                if (_disposed) return;
                _disposed = true;
                list = _list.ToArray();
                _list.Clear();
            }

            foreach (var viewModel in list)
            {
                viewModel.Dispose();
            }
        }

        private async Task<ToDoViewModel[]> GetCurrent()
        {
            var list = await _toDoService.ToDoList;
            lock (_lockObj)
            {
                var addList = list.Where(todo => _list.All(model => todo.Id != model.Id))
                    .Select(todo => new ToDoViewModel(todo, _toDoService)).ToArray();
                var removeList = _list.Where(model => list.All(todo => todo.Id != model.Id)).ToArray();
                foreach (var toDoViewModel in addList)
                {
                    _list.Add(toDoViewModel);
                    _addTodo.OnNext(toDoViewModel);
                }

                foreach (var toDoViewModel in removeList)
                {
                    _list.Remove(toDoViewModel);
                    _removeTodo.OnNext(toDoViewModel);
                }

                return _list.ToArray();
            }
        }

        private void OnNextAdded(ToDo value)
        {
            lock (_lockObj)
            {
                if (_list.FirstOrDefault(model => model.Id == value.Id) != null) return;
                var toDoViewModel = new ToDoViewModel(value, _toDoService);
                _list.Add(toDoViewModel);
                _addTodo.OnNext(toDoViewModel);
            }
        }

        private void OnNextRemoved(ToDo value)
        {
            lock (_lockObj)
            {
                var removeTodo = _list.FirstOrDefault(model => model.Id == value.Id);
                if (removeTodo == null) return;
                _list.Remove(removeTodo);
                _removeTodo.OnNext(removeTodo);
            }
        }
    }
}