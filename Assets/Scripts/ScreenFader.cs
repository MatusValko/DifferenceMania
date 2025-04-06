using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public float fadeDuration = 0.5f;

    void Start()
    {
        // StartCoroutine(FadeIn());
    }

    public void FadeOut(int scene)
    {
        StartCoroutine(_fadeOut(scene));
    }
    public void FadeIn()
    {
        StartCoroutine(_fadeIn());
    }


    private IEnumerator _fadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeGroup.alpha = t / fadeDuration;
            yield return null;
        }
        fadeGroup.alpha = 0;
        fadeGroup.blocksRaycasts = false; // Prevent interaction during fade out

    }

    private IEnumerator _fadeOut(int scene)
    {
        fadeGroup.blocksRaycasts = true; // Prevent interaction during fade out
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = t / fadeDuration;
            yield return null;
        }

        fadeGroup.alpha = 1;
        SceneManager.LoadScene(scene);
    }
}
