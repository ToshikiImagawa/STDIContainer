// Copyright (c) 2022 COMCREATE. All rights reserved.

using System;

namespace SampleApp.Model
{
    public struct ToDo : IEquatable<ToDo>
    {
        public ToDo(
            int id,
            ToDoStatus status,
            string message
        )
        {
            Id = id;
            Status = status;
            Message = message;
        }

        public int Id { get; }
        public ToDoStatus Status { get; }
        public string Message { get; }

        public bool Equals(ToDo other)
        {
            return Id == other.Id &&
                   Status == other.Status &&
                   Message == other.Message;
        }

        public override bool Equals(object obj)
        {
            return obj is ToDo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (int)Status;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}