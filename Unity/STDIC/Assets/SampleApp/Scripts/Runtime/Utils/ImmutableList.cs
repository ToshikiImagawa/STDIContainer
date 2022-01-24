using System;
using System.Linq;
using SampleApp.Model;

namespace SampleApp.Utils
{
    public class ImmutableList<T>
    {
        private static readonly ImmutableList<T> Empty = new ImmutableList<T>();
        private readonly IObserver<T>[] _data;
        public IObserver<T>[] Data => _data;

        public ImmutableList<T> Add(IObserver<T> value)
        {
            var newData = new IObserver<T>[_data.Length + 1];
            Array.Copy(_data, newData, _data.Length);
            newData[_data.Length] = value;
            return new ImmutableList<T>(newData);
        }

        public ImmutableList<T> Remove(IObserver<T> value)
        {
            var i = IndexOf(value);
            if (i < 0) return this;

            var length = _data.Length;
            if (length == 1) return Empty;

            var newData = new IObserver<T>[length - 1];

            Array.Copy(_data, 0, newData, 0, i);
            Array.Copy(_data, i + 1, newData, i, length - i - 1);

            return new ImmutableList<T>(newData);
        }

        private int IndexOf(IObserver<T> value)
        {
            for (var i = 0; i < _data.Length; ++i)
            {
                if (Equals(_data[i], value)) return i;
            }

            return -1;
        }

        public ImmutableList()
        {
            _data = Array.Empty<IObserver<T>>();
        }

        public ImmutableList(IObserver<T>[] data)
        {
            _data = data.Where(observer => observer != null).ToArray();
        }
    }
}