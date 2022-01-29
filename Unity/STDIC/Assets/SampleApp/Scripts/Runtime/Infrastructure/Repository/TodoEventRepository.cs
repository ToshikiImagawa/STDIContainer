// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using SampleApp.Domain.Repository;
using SampleApp.Model;
using SampleApp.Utils;

namespace SampleApp.Infrastructure.Repository
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TodoEventRepository : ITodoEventRepository
    {
        private readonly Subject<ToDo> _updateSubject = new Subject<ToDo>();
        private readonly Subject<ToDo> _addedSubject = new Subject<ToDo>();
        private readonly Subject<ToDo> _removedSubject = new Subject<ToDo>();

        public void Updated(ToDo todo)
        {
            _updateSubject.OnNext(todo);
        }

        public void Added(ToDo todo)
        {
            _addedSubject.OnNext(todo);
        }

        public void Removed(ToDo todo)
        {
            _removedSubject.OnNext(todo);
        }

        public IObservable<ToDo> OnUpdatedToDo => _updateSubject;
        public IObservable<ToDo> OnAddedToDo => _addedSubject;
        public IObservable<ToDo> OnRemovedToDo => _removedSubject;
    }
}