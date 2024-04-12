using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image fadeImage;
    float fadeTime = 2.0f;
    float currentTime = 0.0f;

    ScreenManager screenManager; // constructor for ScreenManager takes in a MonoBehaviour as a parameter

    void Start()
    {
        // constructor for ScreenManager takes in a MonoBehaviour as a parameter
        screenManager = new ScreenManager(this);

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            // FadeIn();
            screenManager.FadeIn(fadeImage, fadeTime);
        }
    }

    public void StartGame()
    {
        screenManager.FadeOut(fadeImage, fadeTime);
        PlayerPrefs.SetInt("Money", 3);
        // FadeOut();
        Invoke("NextScene", fadeTime + .2f);
    }

    void NextScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        LevelManager.ForceLevel("KrabOutside");
    }

    public void QuitGame()
    {
        Application.Quit();
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
