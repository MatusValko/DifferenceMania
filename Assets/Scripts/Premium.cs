using System.Collections;
using System.Collections.Generic;
using ntw.CurvedTextMeshPro;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Premium : MonoBehaviour
{
    public GameObject Content;

    public Animator[] CoinAnimators;

    public Image SelectedCoinImage;

    public Material spriteMaterial;

    [SerializeField] private TextMeshProUGUI _coinsText;


    private float shinePositon = 0;
    float shineSpeed = 1f;
    // private Coroutine shineRoutine = null;

    IEnumerator PlayAnimationAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        List<Animator> selectedElements = new List<Animator>();
        // Randomly select three elements
        while (selectedElements.Count < 3)
        {
            int randomIndex = Random.Range(0, CoinAnimators.Length);
            if (!selectedElements.Contains(CoinAnimators[randomIndex]))
            {
                selectedElements.Add(CoinAnimators[randomIndex]);
            }
        }

        for (int i = 0; i < selectedElements.Count; i++)
        {
            // Debug.Log("Shine" + i);

            StartCoroutine(PlayAnimation(selectedElements[i]));

            float startTime = Time.time;
            SelectedCoinImage = selectedElements[i].gameObject.transform.Find("CoinsImage").GetComponent<Image>();

            SelectedCoinImage.material = spriteMaterial;

            while (Time.time < startTime + 1f / shineSpeed)
            {
                shinePositon = ShineCurve((Time.time - startTime) * shineSpeed);
                // Debug.Log("Position: " + shinePositon);
                // SelectedCoinMaterial.SetFloat("_ShineLocation", shinePositon);


                spriteMaterial.SetFloat("_ShineLocation", shinePositon);
                //DO NOT REMOVE, SHINE WILL NOT RENDER!
                SelectedCoinImage.enabled = false;
                SelectedCoinImage.enabled = true;

                // SelectedCoinImage.material = emptyMaterial;
                yield return new WaitForEndOfFrame();
            }

            SelectedCoinImage.material = null;
            yield return new WaitForSeconds(1.5f);
        }

    }

    IEnumerator PlayAnimation(Animator animator)
    {
        // Play animation from New Layer
        // animator.Play("Shine", 1);
        //set trigger Shine
        animator.SetTrigger("Shine");
        yield break;
    }
    void OnEnable()
    {
        _updateUI();
        //GO TO TOP OF WINDOW
        Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        TextProOnACircle component = GetComponentInChildren<TextProOnACircle>();
        // turn off and on the component to force it to update the curvature
        if (component != null)
        {
            StartCoroutine(refreshTextProOnACircle(component));
        }
        StartCoroutine(PlayAnimationAfterDelay());
    }

    private void _updateUI()
    {
        _coinsText.text = $"{GameManager.Instance.GetCoins()}";
    }

    private IEnumerator refreshTextProOnACircle(TextProOnACircle component)
    {
        TextMeshProUGUI text = component.GetComponent<TextMeshProUGUI>();
        Color tempColor = text.color;
        tempColor.a = 0f; // Set alpha to 0 to force the component to update
        text.color = tempColor;
        // DebugLogger.Log("TextProOnACircle component found and re-enabled.");
        float waitedFrames = 0f;
        while (tempColor.a <= 1f)
        {
            waitedFrames += 0.001f;
            tempColor.a += waitedFrames; // Set alpha to 0 to force the component to update
            text.color = tempColor;
            // component.enabled = false; // Disable the component
            // component.enabled = true; // Re-enable the component
            yield return new WaitForEndOfFrame(); // Wait for the end of the frame
        }
    }

    void OnDisable()
    {
        // Debug.Log("PrintOnDisable: script was disabled");
        spriteMaterial.SetFloat("_ShineLocation", 0f);
        if (SelectedCoinImage != null)
        {
            SelectedCoinImage.material = null;
        }
    }
    private float ShineCurve(float lerpProgress)
    {
        float newValue = lerpProgress * lerpProgress * lerpProgress * (lerpProgress * (6f * lerpProgress - 15f) + 10f);
        return newValue;
    }
}
