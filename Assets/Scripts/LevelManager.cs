using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ObjectiveParam
{
    public ObjectiveType oType;
    public int oCount;
}
public class LevelManager : MonoBehaviour
{
    //todo: make static to prevent LevelManager instance retrieval

    public ObjectiveParam[] objectiveParams;

    static List<Objective> objectiveList = new List<Objective>();

    public static Objective currObjective;

    public float levelDuration = 10f;
    float countDown;
    public static bool isGameOver = false;
    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;

    public string nextLevel;

    public float money = 0;

    UIController ui;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        countDown = levelDuration;
        //gameText.gameObject.SetActive(false);
        ui = GameObject.FindWithTag("UI").GetComponent<UIController>();
        ui.SetTimerText(countDown);
        initObjectiveList();
        SetCurrentObjective();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            
            if (currObjective.CheckAchieved())
            {
                Debug.Log("Objective Achieved!");
                SetCurrentObjective();
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

            ui.SetTimerText(countDown);
            ui.SetScoreText(money);
        }
    }

    private void initObjectiveList()
    {
        
        foreach (ObjectiveParam op in objectiveParams) 
        {
            switch (op.oType)
            {
                case ObjectiveType.MONEY:
                    objectiveList.Add(new MoneyObjective(op.oCount));
                    break;
                case ObjectiveType.INTERACTION:
                    objectiveList.Add(new InteractObjective(op.oCount));
                    break;
                default:
                    break;
            }
            
        }
        
    }

    void SetCurrentObjective()
    {
        
        if (objectiveList.Count > 0)
        {
            //pop first objective out of list and into current objectives
            currObjective = objectiveList[0];
            objectiveList.RemoveAt(0);
            Debug.Log("NEW OBJECTIVE OF TYPE: " + currObjective.objType);
        }
        else 
        {
            //all objectives complete, so level is complete!
            LevelBeat();
        }
        
        /*
        switch (objType)
        {
            case ObjectiveType.MONEY:
                objective = new MoneyObjective(objectiveTargetCount);
                ui.SetGameText("Objective: Collect $" + objectiveTargetCount + ".");
                Debug.Log("GOAL: MONEY");
                break;
            default:
                objective = new InteractObjective();
                ui.SetGameText("Objective: Enter the Krusty Krab.");
                Debug.Log("GOAL: ENTER");
                break;
        }
        */
    }


    public void LevelLost()
    {
        isGameOver = true;
        ui.SetGameText("You Lost!");

        if (gameOverSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 0.5f;
            AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);
        }

        Invoke("LoadCurrentLevel", 2);
    }

    public void LevelBeat()
    {
        isGameOver = true;
        ui.SetGameText("You Won!");
    
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
