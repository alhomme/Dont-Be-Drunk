using UnityEngine;

public abstract class StaticScriptableObject<T> : ScriptableObject
{
    [SerializeField] private T mValue;

    public T Value
    {
        get { return mValue; }
    }
}
