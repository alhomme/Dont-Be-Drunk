using System.Collections.Generic;
using UnityEngine;

public static class MyUtils
{
    public static void Log(string message)
    {
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
}
