using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UISlider 
{
    RELOAD,
    DASH,
    HOMING,
    GRENADE,
    PULSE


}
public class UIController : MonoBehaviour
{
    public Text timerText, scoreText, gameText;

    public Image messagePanel, objectivePanel;


    public Slider reloadSlider, dashSlider, homingSlider, grenadeSlider, pulseSlider;

    Slider currSlider;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetSlider(UISlider whichSlider, float value)
    {
        switch (whichSlider)
        {
            case UISlider.RELOAD:
                currSlider = reloadSlider;
                break;
            case UISlider.DASH:
                currSlider = dashSlider;
                break;
            case UISlider.HOMING:
                currSlider = homingSlider;
                break;
            case UISlider.GRENADE:
                currSlider = grenadeSlider;
                break;
            case UISlider.PULSE:
                currSlider = pulseSlider;
                break;

        }
        currSlider.value = value;
    }

    public void SetTimerText(float countDown)
    {
        timerText.text = countDown.ToString("f2");
    }

    public void SetScoreText(float money)
    {
        scoreText.text = "$" + money.ToString();
    }

    public void SetGameText(string message)
    {
        gameText.text = message;
    }

    public void CheckSlidersActive()
    {
        //check whether player has powerups; if so, activates that powerup slider 
        homingSlider.gameObject.SetActive(FlagManager.playerHasHoming);
        grenadeSlider.gameObject.SetActive(FlagManager.playerHasGrenades);
        pulseSlider.gameObject.SetActive(FlagManager.playerHasPulse);
    }

    public void SetObjectiveArrow(Objective obj)
    {
        if (obj.objType != ObjectiveType.INTERACTION 
            || obj.interactable == null
            || LevelManager.isGameOver)
        {
            objectivePanel.gameObject.SetActive(false);
        }
        else
        {
            objectivePanel.gameObject.SetActive(true);
            objectivePanel.transform.eulerAngles = new Vector3(
                90f, 
                0f, 
                -Vector3.SignedAngle(
                    transform.forward, 
                    (obj.interactable.transform.position - player.transform.position),
                    Vector3.up
                    )
                );
        }
    }
}
