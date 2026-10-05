using System;
using UnityEngine;

public abstract class RuntimeScriptableObject<T> : ScriptableObject
{
    private T mValue;
    public event Action<T> OnChanged;

    public T Value
    {
        get { return mValue; }
        set
        {
            mValue = value;
            OnChanged?.Invoke(mValue);
        }
    }
}
