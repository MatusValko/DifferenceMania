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
    public static void Log(object logMsg)
    {
        UnityEngine.Debug.Log(logMsg);
    }

    [Conditional(DefineSymbol)]
    public static void LogWarning(object logMsg)
    {
        UnityEngine.Debug.LogWarning(logMsg);
    }

    [Conditional(DefineSymbol)]
    public static void LogError(object logMsg)
    {
        UnityEngine.Debug.LogError(logMsg);
    }

}
