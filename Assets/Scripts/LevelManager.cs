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

    public Text timerText, scoreText, gameText;

    public Image messagePanel;
    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;

    public string nextLevel;

    public static float score = 0;


    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        countDown = levelDuration;
        //gameText.gameObject.SetActive(false);
        SetTimerText();
        messagePanel.gameObject.SetActive(true);
        gameText.text = "Objective: Collect $10.";
        messagePanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            
            if (score >= 10)
            {
                LevelBeat();
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
            SetScoreText();
        }
    }

    void SetTimerText()
    {
        timerText.text = countDown.ToString("f2");
    }

    void SetScoreText()
    {
        scoreText.text = "$" + score.ToString();
    }

    public void LevelLost()
    {
        isGameOver = true;
        gameText.text = "You Lost!";
        messagePanel.gameObject.SetActive(true);

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
        messagePanel.gameObject.SetActive(true);
    
        if (gameWonSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 2;
            AudioSource.PlayClipAtPoint(gameWonSFX, Camera.main.transform.position);
        }
    }

    void LoadLevel() {
        SceneManager.LoadScene(nextLevel);
    }
}
