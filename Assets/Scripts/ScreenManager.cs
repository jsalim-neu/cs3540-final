using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
public class ScreenManager : MonoBehaviour
{
    public static Image fadeImage;
    public float fadeTime = 2.0f;
    float currentTime = 0.0f;

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            FadeOut();
        }
    }

    public void FadeOut()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, currentTime / fadeTime));
            yield return null;
        }
        currentTime = 0.0f;
    }

    IEnumerator FadeInCoroutine()
    {
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, currentTime / fadeTime));
            yield return null;
        }
        currentTime = 0.0f;
        fadeImage.gameObject.SetActive(false);
    }
}
*/

public class ScreenManager
{
    MonoBehaviour monoBehaviour;

    public ScreenManager(MonoBehaviour _monoBehaviour)
    {
        monoBehaviour = _monoBehaviour;
    }

    public void FadeOut(Image fadeImage, float fadeTime = 2.0f)
    {
        float currentTime = 0.0f;
        fadeImage.gameObject.SetActive(true);

        monoBehaviour.StartCoroutine(
            FadeOutCoroutine(fadeImage, currentTime, fadeTime));
    }

    public void FadeIn(Image fadeImage, float fadeTime = 2.0f)
    {
        float currentTime = 0.0f;
        fadeImage.gameObject.SetActive(true);
        monoBehaviour.StartCoroutine(
            FadeInCoroutine(fadeImage, currentTime, fadeTime));
    }

    IEnumerator FadeOutCoroutine(Image fadeImage, float currentTime, float fadeTime = 2.0f)
    {
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, currentTime / fadeTime));
            yield return null;
        }

        currentTime = 0.0f;
    }

    IEnumerator FadeInCoroutine(Image fadeImage, float currentTime, float fadeTime = 2.0f)
    {
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, currentTime / fadeTime));
            yield return null;
        }

        currentTime = 0.0f;
        fadeImage.gameObject.SetActive(false);
    }
}
