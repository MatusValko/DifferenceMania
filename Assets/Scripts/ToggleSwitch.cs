using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// using UnityEngine.UIElements;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    [Header("Slider setup")]
    [SerializeField, Range(0, 1f)]
    protected float sliderValue;
    public bool CurrentValue { get; private set; } //ON or OFF

    private bool _previousValue;
    private Slider _slider;

    [Header("Animation")]
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.5f;
    [SerializeField]
    private AnimationCurve slideEase =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine _animateSliderCoroutine;

    [Header("Events")]
    [SerializeField] private UnityEvent onToggleOn;
    [SerializeField] private UnityEvent onToggleOff;

    [Header("TextFields")]
    [SerializeField] private GameObject _ON;
    [SerializeField] private GameObject _OFF;

    [Header("Handle")]
    [SerializeField] private Image _handle;
    [SerializeField] private Sprite _greySlider;
    [SerializeField] private Sprite _greenSlider;


    // private ToggleSwitchGroupManager _toggleSwitchGroupManager;

    protected Action transitionEffect;

    protected virtual void OnValidate()
    {
        SetupToggleComponents();

        _slider.value = sliderValue;
    }

    private void SetupToggleComponents()
    {
        if (_slider != null)
            return;

        SetupSliderComponent();
    }

    private void SetupSliderComponent()
    {
        _slider = GetComponent<Slider>();

        if (_slider == null)
        {
            Debug.Log("No slider found!", this);
            return;
        }

        _slider.interactable = false;
        var sliderColors = _slider.colors;
        sliderColors.disabledColor = Color.white;
        _slider.colors = sliderColors;
        _slider.transition = Selectable.Transition.None;



    }

    // public void SetupForManager(ToggleSwitchGroupManager manager)
    // {
    //     _toggleSwitchGroupManager = manager;
    // }


    protected virtual void Awake()
    {
        SetupSliderComponent();
    }
    void Start()
    {
        if (SoundManager.Instance == null)
        {
            DebugLogger.LogWarning("SoundManager is not present in the scene!");
            return;
        }

        if (gameObject.name == "MusicSwitch")
        {
            CurrentValue = SoundManager.Instance.IsMusicMuted();
        }
        else if (gameObject.name == "SoundSwitch")
        {
            CurrentValue = SoundManager.Instance.IsSFXMuted();
        }

        if (!CurrentValue)
        {
            Toggle();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }


    private void Toggle()
    {
        SetStateAndStartAnimation(!CurrentValue);

        // if (_toggleSwitchGroupManager != null)
        //     _toggleSwitchGroupManager.ToggleGroup(this);
        // else
    }

    private void SetStateAndStartAnimation(bool state)
    {
        _previousValue = CurrentValue;
        CurrentValue = state;



        if (_previousValue != CurrentValue)
        {
            if (CurrentValue)
            {
                onToggleOn?.Invoke();

                _handle.sprite = _greenSlider;
                _ON.SetActive(true);
                _OFF.SetActive(false);

                SwitchSwitch();

            }
            else
            {
                onToggleOff?.Invoke();
                _handle.sprite = _greySlider;
                _ON.SetActive(false);
                _OFF.SetActive(true);

                SwitchSwitch();
            }
        }

        if (_animateSliderCoroutine != null)
            StopCoroutine(_animateSliderCoroutine);

        _animateSliderCoroutine = StartCoroutine(AnimateSlider());
    }

    private void SwitchSwitch()
    {
        if (gameObject.name == "MusicSwitch")
        {
            SoundManager.Instance.ToggleMuteMusic(!CurrentValue);
        }
        else if (gameObject.name == "SoundSwitch")
        {
            SoundManager.Instance.ToggleMuteSFX(!CurrentValue);

        }
    }

    private IEnumerator AnimateSlider()
    {
        float startValue = _slider.value;
        float endValue = CurrentValue ? 1 : 0;

        float time = 0;
        if (animationDuration > 0)
        {
            while (time < animationDuration)
            {
                time += Time.deltaTime;

                float lerpFactor = slideEase.Evaluate(time / animationDuration);
                _slider.value = sliderValue = Mathf.Lerp(startValue, endValue, lerpFactor);

                transitionEffect?.Invoke();

                yield return null;
            }
        }

        _slider.value = endValue;
    }

    void OnEnable()
    {

    }
}