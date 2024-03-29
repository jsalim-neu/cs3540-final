using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Objective objective;

    public ObjectiveType objType;

    public int objectiveTargetCount = 10;
    public float levelDuration = 10f;
    float countDown;
    public static bool isGameOver = false;

    public Text timerText, scoreText, gameText;

    public Image messagePanel;
    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;

    public string nextLevel;

    public float money = 0;



    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        countDown = levelDuration;
        //gameText.gameObject.SetActive(false);
        SetTimerText();
        messagePanel.gameObject.SetActive(true);
        SetObjective();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            if (objective.CheckAchieved())
            {
                Debug.Log(objective.objectiveCounter + ", " + objective.objectiveCountGoal);
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
        scoreText.text = "$" + money.ToString();
    }

    void SetObjective()
    {
        switch (objType)
        {
            case ObjectiveType.MONEY:
                objective = new MoneyObjective(objectiveTargetCount);
                gameText.text = "Objective: Collect $" + objectiveTargetCount + ".";
                Debug.Log("GOAL: MONEY");
                break;
            default:
                objective = new InteractObjective();
                gameText.text = "Objective: Enter the Krusty Krab.";
                Debug.Log("GOAL: ENTER");
                break;
        }
    }

    public void LevelLost()
    {
        isGameOver = true;
        gameText.text = "You Lost!";
        messagePanel.gameObject.SetActive(true);

        if (gameOverSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 0.5f;
            AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);
        }

        Invoke("LoadCurrentLevel", 2);
    }

    public void LevelBeat()
    {
        isGameOver = true;
        gameText.text = "You Won!";
        messagePanel.gameObject.SetActive(true);
    
        if (gameWonSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 1;
            AudioSource.PlayClipAtPoint(gameWonSFX, Camera.main.transform.position);
        }
        if (nextLevel != "") 
        {
            Invoke("LoadNextLevel", 2);
        }

    }


    void LoadCurrentLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadNextLevel() {
        SceneManager.LoadScene(nextLevel);
    }
}
