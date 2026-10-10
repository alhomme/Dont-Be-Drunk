using System.Collections.Generic;
using UnityEngine;

public static class MyUtils
{
    public static void Log(string message, bool print = true)
    {
        if (print)
            Debug.Log(message);
    }

    public static void LogList(List<int> intList)
    {
        string str = "List = { ";

        foreach (var i in intList)
        {
            str += i.ToString();
            str += " ";
        }
        str += "}";

        Log(str);
    }

    public static void LogList(List<Vector2> vec2List)
    {
        string str = "List = { ";

        foreach (var i in vec2List)
        {
            str += i.ToString();
            str += " ";
        }
        str += "}";

        Log(str);
    }
}
