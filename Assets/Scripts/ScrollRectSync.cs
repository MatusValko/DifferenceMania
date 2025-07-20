using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SmartBidirectionalScrollSync : MonoBehaviour
{
    public ScrollRect scrollRectA;
    public ScrollRect scrollRectB;

    private bool syncing = false;
    private bool userIsDraggingA = false;
    private bool userIsDraggingB = false;

    private bool _isZooming = false;

    void Start()
    {
        scrollRectA.onValueChanged.AddListener(OnScrollA);
        scrollRectB.onValueChanged.AddListener(OnScrollB);
    }

    void Update()
    {
        userIsDraggingA = IsDragging(scrollRectA);
        userIsDraggingB = IsDragging(scrollRectB);
    }

    bool IsDragging(ScrollRect scrollRect)
    {
        return scrollRect != null &&
               scrollRect.IsActive() &&
                  scrollRect.velocity != Vector2.zero &&
               Input.GetMouseButton(0); // works for both mouse & touch
    }

    void OnScrollA(Vector2 pos)
    {
        _isZooming = DifferencesManager.Instance._isZooming;
        if (_isZooming || syncing || !userIsDraggingA) return;

        syncing = true;
        scrollRectB.normalizedPosition = pos;
        syncing = false;
    }

    void OnScrollB(Vector2 pos)
    {
        _isZooming = DifferencesManager.Instance._isZooming;
        if (_isZooming || syncing || !userIsDraggingB) return;

        syncing = true;
        scrollRectA.normalizedPosition = pos;
        syncing = false;
    }
}
