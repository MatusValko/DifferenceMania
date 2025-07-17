using UnityEngine;
using UnityEngine.UI;

public class BidirectionalScrollSync : MonoBehaviour
{
    public ScrollRect scrollRectA;
    public ScrollRect scrollRectB;

    private bool isSyncingA = false;
    private bool isSyncingB = false;

    void Start()
    {
        scrollRectA.onValueChanged.AddListener(OnScrollA);
        scrollRectB.onValueChanged.AddListener(OnScrollB);
    }

    void OnScrollA(Vector2 pos)
    {
        if (isSyncingA) return;

        isSyncingB = true;
        scrollRectB.normalizedPosition = pos;
        isSyncingB = false;
    }

    void OnScrollB(Vector2 pos)
    {
        if (isSyncingB) return;

        isSyncingA = true;
        scrollRectA.normalizedPosition = pos;
        isSyncingA = false;
    }
}
