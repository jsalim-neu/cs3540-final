using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public float levelDuration = 10f;
    float countDown;
    public static bool isGameOver = false;
    public static int pickUpCount;

    public Text timerText;
    public Text gameText;

    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;

    public string nextLevel;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        pickUpCount = GameObject.FindGameObjectsWithTag("PickUp").Length;
        countDown = levelDuration;
        SetTimerText();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            pickUpCount = GameObject.FindGameObjectsWithTag("PickUp").Length;

            if (pickUpCount <= 0)
            {
                isGameOver = true;
            }

            if (countDown > 0)
            {
                countDown -= Time.deltaTime;
            }
            else
            {
                countDown = 0;
                LevelLost();
            }

            SetTimerText();
        }
    }

    void SetTimerText()
    {
        timerText.text = countDown.ToString("f2");
    }

    public void LevelLost()
    {
        isGameOver = true;
        gameText.text = "You Lost!";
        gameText.gameObject.SetActive(true);

        if (gameOverSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 1;
            AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);
        }

        Invoke("LoadLevel", 2);
    }

    public void LevelBeat()
    {
        isGameOver = true;
        gameText.text = "You Won!";
        gameText.gameObject.SetActive(true);
    
        if (gameWonSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 2;
            AudioSource.PlayClipAtPoint(gameWonSFX, Camera.main.transform.position);
        }
    }

    void LoadLevel() {
        SceneManager.LoadScene(nextLevel);
    }
}
