#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class FontReplacer : MonoBehaviour
{
    [MenuItem("Tools/Replace TMP Fonts In Scene")]
    static void ReplaceTMPFontsInScene()
    {
        TMP_FontAsset LuckiestGuyRegular = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/LuckiestGuy-Regular SDF.asset");
        TMP_FontAsset LuckiestGuyRegularBlackOutline = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/LuckiestGuy-Regular SDF BLACK OUTLINE.asset");
        TMP_FontAsset LuckiestGuyRegularBlackOutlineSmall = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/LuckiestGuy-Regular SDF BLACK OUTLINE SMALL.asset");
        TMP_FontAsset NunitoBold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Nunito-Bold SDF.asset");

        if (LuckiestGuyRegular == null)
        {
            Debug.LogError("Font not found at the specified path!");
            return;
        }
        if (NunitoBold == null)
        {
            Debug.LogError("Font not found at the specified path!");
            return;
        }
        if (LuckiestGuyRegularBlackOutline == null)
        {
            Debug.LogError("Font not found at the specified path!");
            return;
        }
        if (LuckiestGuyRegularBlackOutlineSmall == null)
        {
            Debug.LogError("Font not found at the specified path!");
            return;
        }


        var allText = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        int count = 0;

        foreach (var text in allText)
        {
            Undo.RecordObject(text, "Change TMP Font");
            if (text == null || text.font == null)
            {
                DebugLogger.LogWarning($"TextMeshProUGUI component is null or has no font assigned: {text.name}");
                continue;
            }
            if (text.font.name == "Admisi Display SSi SDF NORMAL")
            {
                DebugLogger.Log($"Changing font {text.font.name} to {LuckiestGuyRegular.name}");
                text.font = LuckiestGuyRegular;
                count++;
            }
            else if (text.font.name == "Admisi Display SSi SDF BLACK OUTLINE")
            {
                DebugLogger.Log($"Changing font {text.font.name} to {LuckiestGuyRegularBlackOutline.name}");
                text.font = LuckiestGuyRegularBlackOutline;
                count++;
            }
            else if (text.font.name == "Admisi Display SSi SDF BLACK OUTLINE SMALL")
            {
                DebugLogger.Log($"Changing font {text.font.name} to {LuckiestGuyRegularBlackOutlineSmall.name}");
                text.font = LuckiestGuyRegularBlackOutlineSmall;

                count++;

            }
            else if (text.font.name == "GillsansMT SDF")
            {
                DebugLogger.Log($"Changing font {text.font.name} to {NunitoBold.name}");
                text.font = NunitoBold;
                count++;
            }
            else if (text.font.name == "LiberationSans SDF")
            {
                DebugLogger.Log($"Changing font {text.font.name} to {LuckiestGuyRegular.name}");
                text.font = LuckiestGuyRegular;
                count++;
            }
        }

        Debug.Log($"Updated {count} TMP text components.");
    }
}
#endif
