// Copyright (c) 2022 COMCREATE. All rights reserved.

using System.Threading.Tasks;
using SampleApp.Model;

namespace SampleApp.Domain.Repository
{
    public interface IToDoRepository
    {
        public Task<ToDo> Create(string message);
        public Task<ToDo> Read(int id);
        public Task<ToDo[]> ReadAll();
        public Task<bool> Update(ToDo newTodo);
        public Task<bool> Delete(int id);
    }
}