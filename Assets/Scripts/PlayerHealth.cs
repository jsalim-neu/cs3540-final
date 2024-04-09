using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 10;

    PlayerController controller;
    //public AudioClip deadSFX;
    public Slider healthSlider;
    public LevelManager levelManager;

    public int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damageAmount;
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            PlayerDies();
        }
    }

    public void TriggerKnockback(Vector3 moveDirection)
    {
        controller.Knockback(moveDirection);
    }

    public void TakeHealth(int healthAmount)
    {
        if (currentHealth < startingHealth)
        {
            currentHealth += healthAmount;
            healthSlider.value = Mathf.Clamp(currentHealth, 0, startingHealth);
        }
    }

    void PlayerDies()
    {
        //AudioSource.PlayClipAtPoint(deadSFX, transform.position);
        transform.Rotate(-90, 0, 0, Space.Self);
        levelManager.LevelLost();
    }
}
