// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SampleApp.Utils;
using SampleApp.Domain.Repository;
using SampleApp.Infrastructure.Helper;
using SampleApp.Model;
using STDIC;
using Random = System.Random;

namespace SampleApp.Infrastructure.Repository
{
    public class ToDoLocalRepository : IToDoRepository
    {
        private readonly LocalDataBaseHelper _helper;
        private readonly object _lockObj = new object();
        private readonly SynchronizationContext _synchronizationContext;

        [Inject]
        public ToDoLocalRepository(LocalDataBaseHelper helper)
        {
            _helper = helper;
            _synchronizationContext = SynchronizationContext.Current;
        }

        public Task<ToDo> Create(string message)
        {
            return _synchronizationContext.Post(() =>
            {
                lock (_lockObj)
                {
                    var todoList = _helper.Load().ToList();
                    var newId = int.Parse(GenerateRandom(6));
                    var todo = new ToDo(newId, ToDoStatus.Open, message);
                    todoList.Add(todo);
                    return _helper.Save(todoList.ToArray()) ? todo : default;
                }
            });
        }

        public Task<ToDo> Read(int id)
        {
            return _synchronizationContext.Post(() =>
            {
                lock (_lockObj)
                {
                    return _helper.Load().FirstOrDefault(todo => todo.Id == id);
                }
            });
        }

        public Task<ToDo[]> ReadAll()
        {
            return _synchronizationContext.Post(() =>
            {
                lock (_lockObj)
                {
                    return _helper.Load();
                }
            });
        }

        public Task<bool> Update(ToDo newTodo)
        {
            return _synchronizationContext.Post(() =>
            {
                lock (_lockObj)
                {
                    var todoList = _helper.Load();
                    for (var index = 0; index < todoList.Length; index++)
                    {
                        var todo = todoList[index];
                        if (todo.Id != newTodo.Id) continue;
                        todoList[index] = newTodo;
                        _helper.Save(todoList);
                        return true;
                    }
                }

                return false;
            });
        }

        public Task<bool> Delete(int id)
        {
            return _synchronizationContext.Post(() =>
            {
                lock (_lockObj)
                {
                    var todoList = _helper.Load();
                    if (todoList.All(todo => todo.Id != id)) return false;
                    _helper.Save(todoList.Where(todo => todo.Id != id).ToArray());
                    return true;
                }
            });
        }

        private static string GenerateRandom(int len)
        {
            var rtn = new StringBuilder();
            var r = UniqueRandom;
            while (rtn.Length < len)
            {
                rtn.Append(r.Next(0, 10));
            }

            return rtn.ToString();
        }

        private static Random UniqueRandom => new Random(RandomSeed);

        private static int RandomSeed
        {
            get
            {
                var bytes = new byte[4];
                var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
                rng.GetBytes(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
        }
    }
}