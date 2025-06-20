#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class ButtonReplacer : EditorWindow
{
    [MenuItem("Tools/Replace Buttons with UIButtonWithSound")]
    public static void ReplaceButtons()
    {
        var buttons = GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None);
        int replacedCount = 0;

        foreach (var button in buttons)
        {
            GameObject go = button.gameObject;
            var onClickEvents = button.onClick;

            // Store button settings
            Navigation nav = button.navigation;
            ColorBlock colors = button.colors;
            SpriteState spriteState = button.spriteState;
            AnimationTriggers triggers = button.animationTriggers;
            Selectable.Transition transition = button.transition;
            Graphic targetGraphic = button.targetGraphic;
            Image image = button.image;

            // Destroy old Button
            Undo.DestroyObjectImmediate(button);

            // Add new custom button
            var newButton = Undo.AddComponent<UIButtonWithSound>(go);

            // Re-assign values
            newButton.onClick = onClickEvents;
            newButton.navigation = nav;
            newButton.colors = colors;
            newButton.spriteState = spriteState;
            newButton.animationTriggers = triggers;
            newButton.transition = transition;
            newButton.targetGraphic = targetGraphic;
            newButton.image = image;

            replacedCount++;
        }

        Debug.Log($"Replaced {replacedCount} buttons with UIButtonWithSound.");
    }
}
#endif
