using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class DebugLogger
{
    private const string DefineSymbol = "UNITY_EDITOR";


    // private static bool ENABLE_LOGS = true;
    // [Conditional("ENABLE_LOGS")]
    [Conditional(DefineSymbol)]
    public static void Log(string logMsg)
    {
        UnityEngine.Debug.Log(logMsg);
    }

    [Conditional(DefineSymbol)]
    public static void LogWarning(string logMsg)
    {
        UnityEngine.Debug.LogWarning(logMsg);
    }

    [Conditional(DefineSymbol)]
    public static void LogError(string logMsg)
    {
        UnityEngine.Debug.LogError(logMsg);
    }

}
