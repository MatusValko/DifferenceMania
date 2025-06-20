using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonWithSound : Button
{
    // public AudioClip clickSound;
    // private AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        DebugLogger.Log("UIButtonWithSound: Button clicked: " + name);
    }
}
