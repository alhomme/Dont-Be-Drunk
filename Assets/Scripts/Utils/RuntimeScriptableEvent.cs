using System;
using UnityEngine;

public abstract class RuntimeScriptableEvent : ScriptableObject
{
    public event Action Event;

    public void Dispatch()
    {
        Event?.Invoke();
    }
}

public abstract class RuntimeScriptableEvent<T> : ScriptableObject
{
    public event Action<T> Event;

    public void Dispatch(T value)
    {
        Event?.Invoke(value);
    }
}



public abstract class RuntimeScriptableEvent<T, U> : ScriptableObject
{
    public event Action<T, U> Event;

    public void Dispatch(T value1, U value2)
    {
        Event?.Invoke(value1, value2);
    }
}
