using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    [SerializeField] private bool _ignoreVertical;
    [SerializeField] private bool _ignoreHorizontal;

    private RectTransform _rectTransform;
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    private Vector2 _lastScreenSize = new Vector2(0, 0);
    private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        Fit();
    }

    private void Update()
    {
        if (Screen.width != _lastScreenSize.x
            || Screen.height != _lastScreenSize.y
            || Screen.safeArea != _lastSafeArea
            || Screen.orientation != _lastOrientation)
        {
            _lastScreenSize = new Vector2(Screen.width, Screen.height);
            _lastSafeArea = Screen.safeArea;
            _lastOrientation = Screen.orientation;

            Fit();
        }
    }

    private void Fit()
    {
        Rect safeArea = Screen.safeArea;

        if (_ignoreVertical)
        {
            safeArea.y = 0;
            safeArea.height = Screen.height;
        }

        if (_ignoreHorizontal)
        {
            safeArea.x = 0;
            safeArea.width = Screen.width;
        }

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}