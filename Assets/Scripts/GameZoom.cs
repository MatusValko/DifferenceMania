#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameZoom : MonoBehaviour, IScrollHandler
{

    [SerializeField] private Camera _mainCamera;


    private Vector3 initialScale;
    private float zoomSpeed = 0.1f;
    public float maxZoom = 8;
    public float minZoom = 1;

    [SerializeField] private Transform _transformImage1;
    [SerializeField] private Transform _transformImage2;


    void Awake()
    {
        initialScale = transform.localScale;
    }
    public void OnScroll(PointerEventData eventData)
    {
        var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
        var desiredScale = transform.localScale + delta;

        desiredScale = ClampDesiredScale(desiredScale);

        // transform.localScale = desiredScale; 
        _transformImage1.localScale = desiredScale;
        _transformImage2.localScale = desiredScale;

    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale)
    {
        desiredScale = Vector3.Max(initialScale, desiredScale);
        desiredScale = Vector3.Min(initialScale * maxZoom, desiredScale);
        return desiredScale;
    }

}
#endif