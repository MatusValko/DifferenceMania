using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Premium : MonoBehaviour
{
    public GameObject PremiumWindow;
    public GameObject Content;

    public Animator[] CoinAnimators;

    public Image SelectedCoinImage;

    public Material spriteMaterial;
    public Material emptyMaterial;


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
        animator.Play("Shine");
        yield break;
    }
    void OnEnable()
    {
        //GO TO TOP OF WINDOW
        Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        StartCoroutine(PlayAnimationAfterDelay());
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
