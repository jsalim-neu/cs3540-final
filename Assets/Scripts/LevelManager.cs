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
    public string oText;

    public GameObject oInteractable;
}
public class LevelManager : MonoBehaviour
{
    public ObjectiveParam[] objectiveParams;

    static List<Objective> objectiveList = new List<Objective>();

    public static Objective currObjective;

    public float levelDuration = 10f;
    float countDown;
    public static bool isGameOver = false;
    public AudioClip gameOverSFX;
    public AudioClip gameWonSFX;

    public string nextLevel;

    public static int money = 3;

    public Image fadeImage;

    UIController ui;
    ScreenManager screenManager;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        countDown = levelDuration;
        //gameText.gameObject.SetActive(false);
        ui = GameObject.FindWithTag("UI").GetComponent<UIController>();
        screenManager = new ScreenManager(this);

        if (fadeImage != null)
        {
            screenManager.FadeIn(fadeImage, 1.0f);
        }

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
                SetObjectiveText();
            }
            else
            {
                countDown = 0;
                LevelLost();
            }
            ui.CheckSlidersActive();
            ui.SetTimerText(countDown);
            ui.SetScoreText(money);
            ui.SetObjectiveArrow(currObjective);
        }
    }

    private void initObjectiveList()
    {
        Debug.Log("Calling initObjList!");
        objectiveList = new List<Objective>();
        
        foreach (ObjectiveParam op in objectiveParams)
        {
            switch (op.oType)
            {
                case ObjectiveType.MONEY:
                    objectiveList.Add(new MoneyObjective(op.oCount));
                    break;
                case ObjectiveType.INTERACTION:
                    objectiveList.Add(new InteractObjective(op.oCount, op.oText, op.oInteractable));
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
            SetObjectiveText();
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

    void SetObjectiveText() 
    {
        switch (currObjective.objType)
        {
            case ObjectiveType.MONEY:
                ui.SetGameText("Objective: Collect " + currObjective.objectiveCountGoal + 
                    " doubloons. (" + currObjective.objectiveCounter + "/" + currObjective.objectiveCountGoal +")");
                break;
            case ObjectiveType.INTERACTION:
                ui.SetGameText("Objective: " + currObjective.objectiveText);
                break;
        }
    }


    public void LevelLost()
    {
        isGameOver = true;
        ui.SetGameText("Game over!");

        if (gameOverSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 0.5f;
            AudioSource.PlayClipAtPoint(gameOverSFX, Camera.main.transform.position);
        }

        if (fadeImage != null) { Invoke("FadeOut", 2); }
        Invoke("LoadCurrentLevel", 6);
    }

    public void LevelBeat()
    {
        isGameOver = true;
        ui.SetGameText("Level complete!");
    
        if (gameWonSFX != null) {
            Camera.main.GetComponent<AudioSource>().pitch = 1;
            AudioSource.PlayClipAtPoint(gameWonSFX, Camera.main.transform.position);
        }

        if (nextLevel != "") 
        {
            if (fadeImage != null) { Invoke("FadeOut", 2); }
            Invoke("LoadNextLevel", 6);
        }

    }

    void LoadCurrentLevel()
    {
        //when current level is reloaded (i.e. player lost), retrieve saved money count (or default $0)
        money = PlayerPrefs.GetInt("Money", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadNextLevel() {
        //when level is beaten, save player's money count
        PlayerPrefs.SetInt("Money", money);
        SceneManager.LoadScene(nextLevel);
    }

    void FadeOut()
    {
        screenManager.FadeOut(fadeImage, 1.0f);
    }

    public static void ForceLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}
