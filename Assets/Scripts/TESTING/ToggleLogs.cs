using UnityEditor;
using UnityEngine;

//USE UNITY_EDITOR INSTEAD
public class ToggleLogs
{
    private const string DefineSymbol = "DEVELOPMENT_BUILD";

    [MenuItem("Tools/Toggle Logs")]
    public static void ToggleEnableLogs()
    {
        // Get current scripting define symbols for the active build target group
        // BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

        // Sanitize and toggle the symbol
        if (currentSymbols.Contains(DefineSymbol))
        {
            // Remove the symbol if it exists
            currentSymbols = currentSymbols.Replace(DefineSymbol, "").Trim(';');
            Debug.Log($"{DefineSymbol + " disabled"}");
        }
        else
        {
            // Add the symbol if it doesn't exist
            if (!string.IsNullOrEmpty(currentSymbols))
                currentSymbols += ";";

            currentSymbols += DefineSymbol;
            Debug.Log($"{DefineSymbol + " enabled"}");
        }

        // Apply the updated define symbols (ensures valid formatting)
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, currentSymbols.Trim());
    }
}
