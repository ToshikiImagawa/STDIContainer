using System;

namespace STDIC
{
    public interface IConstructor
    {
    }

    public interface IConstructor<out T> : IConstructor
    {
        Type[] GetParameterTypes();
        T New(object[] parameters);
    }
}