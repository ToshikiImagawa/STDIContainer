// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Threading.Tasks;
using SampleApp.Domain.Repository;
using SampleApp.Model;
using STDIC;

namespace SampleApp.Domain.Service
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _repository;
        private readonly ITodoEventRepository _eventRepository;

        [Inject]
        public ToDoService(
            IToDoRepository repository,
            ITodoEventRepository eventRepository
        )
        {
            _repository = repository;
            _eventRepository = eventRepository;
        }

        public async Task<ToDo> Create(string message)
        {
            var todo = await _repository.Create(message);
            _eventRepository.Added(todo);
            return todo;
        }

        public async void UpdateStatus(int id, ToDoStatus status)
        {
            var todo = await _repository.Read(id);
            if (todo.Id != id || todo.Status == status) return;
            var newToDo = new ToDo(
                todo.Id,
                status,
                todo.Message
            );
            var updated = await _repository.Update(newToDo);
            if (updated)
            {
                _eventRepository.Updated(newToDo);
            }
        }

        public async void Delete(int id)
        {
            var todo = await _repository.Read(id);
            if (todo.Id != id) return;
            var deleted = await _repository.Delete(id);
            if (deleted)
            {
                _eventRepository.Removed(todo);
            }
        }

        public Task<ToDo[]> ToDoList => _repository.ReadAll();
        public IObservable<ToDo> OnUpdateToDo => _eventRepository.OnUpdatedToDo;
        public IObservable<ToDo> OnAddedToDo => _eventRepository.OnAddedToDo;
        public IObservable<ToDo> OnRemovedToDo => _eventRepository.OnRemovedToDo;
    }
}