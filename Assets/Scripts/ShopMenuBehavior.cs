using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopMenuBehavior : MonoBehaviour
{
    public AudioClip openShopSFX, buySFX, buyFailSFX;
    public static bool isGamePaused = false;

    public GameObject pauseMenu;

    public static int homingPrice = 2, grenadePrice = 4, pulsePrice = 6;

    [SerializeField] public TextMeshProUGUI moneyText;

    void Start()
    {
        ResumeGame();
    }
    // Update is called once per frame
    void Update()
    {
        if (isGamePaused & Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
        }
        moneyText.text = "$" + LevelManager.money;

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
        PlayClipAtCamera(openShopSFX);

    }

    public void BuyHoming()
    {
        if (FlagManager.playerHasHoming)
        {
            Debug.Log("Player has homing projectiles already!");
            PlayClipAtCamera(buyFailSFX);
        }
        else if (LevelManager.money < homingPrice)
        {
            Debug.Log("Player can't afford homing projectiles!");
            PlayClipAtCamera(buyFailSFX);
        }
        else 
        {
            LevelManager.money -= homingPrice;
            FlagManager.playerHasHoming = true;
            PlayClipAtCamera(buySFX);

        }
    }

    public void BuyGrenades()
    {
        if (FlagManager.playerHasGrenades)
        {
            Debug.Log("Player has grenades already!");
            PlayClipAtCamera(buyFailSFX);
        }
        else if (LevelManager.money < grenadePrice)
        {
            Debug.Log("Player can't afford grenades!");
            PlayClipAtCamera(buyFailSFX);
        }
        else 
        {
            LevelManager.money -= grenadePrice;
            FlagManager.playerHasGrenades = true;
            PlayClipAtCamera(buySFX);

        }
    }

    public void BuyPulse()
    {
        if (FlagManager.playerHasPulse)
        {
            Debug.Log("Player has pulse already!");
            PlayClipAtCamera(buyFailSFX);
        }
        else if (LevelManager.money < pulsePrice)
        {
            Debug.Log("Player can't afford pulse!");
            PlayClipAtCamera(buyFailSFX);
        }
        else 
        {
            LevelManager.money -= pulsePrice;
            FlagManager.playerHasPulse = true;
            PlayClipAtCamera(buySFX);
        }
    }

    void PlayClipAtCamera(AudioClip clip)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);
    }


}
