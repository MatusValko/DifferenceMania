using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonWithSound : Button
{
    // public AudioClip clickSound;
    // private AudioSource audioSource;
    protected override void Awake()
    {
        base.Awake();
        // Add sound listener automatically
        onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        SoundManager.PlaySound(SoundType.BUTTON_CLICK);
        // DebugLogger.Log("UIButtonWithSound: Button clicked: " + name);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // DebugLogger.Log("ONenable");
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // if (CompareTag("NoAnimation"))
            // {
            //     DebugLogger.Log("NoAnimation");
            //     return;
            // }
            animator.keepAnimatorStateOnDisable = true;
            animator.Rebind();
            animator.Update(0f);
        }
    }



    // protected override void Start()
    // {
    //     base.Start();
    // }

    // public override void OnPointerClick(PointerEventData eventData)
    // {
    //     base.OnPointerClick(eventData);

    //     SoundManager.PlaySound(SoundType.BUTTON_CLICK);
    //     DebugLogger.Log("UIButtonWithSound: Button clicked: " + name);
    // }
}
