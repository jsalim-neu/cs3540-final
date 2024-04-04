using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopMenuBehavior : MonoBehaviour
{
    public static bool isGamePaused = false;

    public GameObject pauseMenu;

    public static int homingPrice = 2, grenadePrice = 4, pulsePrice = 6;

    void Start()
    {
        ResumeGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
        }

    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void OpenShop()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    public void BuyHoming()
    {
        if (FlagManager.playerHasHoming)
        {
            Debug.Log("Player has homing projectiles already!");
        }
        else if (LevelManager.money < homingPrice)
        {
            Debug.Log("Player can't afford homing projectiles!");
        }
        else 
        {
            LevelManager.money -= homingPrice;
            FlagManager.playerHasHoming = true;
        }
    }

    public void BuyGrenades()
    {
        if (FlagManager.playerHasGrenades)
        {
            Debug.Log("Player has grenades already!");
        }
        else if (LevelManager.money < grenadePrice)
        {
            Debug.Log("Player can't afford grenades!");
        }
        else 
        {
            LevelManager.money -= grenadePrice;
            FlagManager.playerHasGrenades = true;
        }
    }

    public void BuyPulse()
    {
        if (FlagManager.playerHasPulse)
        {
            Debug.Log("Player has pulse already!");
        }
        else if (LevelManager.money < pulsePrice)
        {
            Debug.Log("Player can't afford pulse!");
        }
        else 
        {
            LevelManager.money -= pulsePrice;
            FlagManager.playerHasPulse = true;
        }
    }


}
